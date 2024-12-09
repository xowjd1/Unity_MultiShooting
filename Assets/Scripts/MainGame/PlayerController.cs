using Fusion;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    // �÷��̾� �Է� ��� ���� üũ
    public bool AcceptAnyInput => PlayerIsAlive && !GameManager.MatchIsOver && !playerChatController.IsTyping;

    // ������Ʈ ����
    [SerializeField] private PlayerChatController playerChatController;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject cam;
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float jumpForce = 1000;

    // ���� üũ ����
    [Header("Grounded Vars")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundDetectionObj;

    // ��Ʈ��ũ ����ȭ ����
    [Networked] public TickTimer RespawnTimer { get; private set; }
    [Networked] public NetworkBool PlayerIsAlive { get; private set; }
    [Networked(OnChanged = nameof(OnNicknameChanged))]
    private NetworkString<_8> playerName { get; set; }
    [Networked] private NetworkButtons buttonsPrev { get; set; }
    [Networked] private Vector2 serverNextSpawnPoint { get; set; }
    [Networked] private NetworkBool isGrounded { get; set; }
    [Networked] private TickTimer respawnToNewPointTimer { get; set; }

    // ���� ����
    private float horizontal;
    private Rigidbody2D rigid;
    private PlayerWeaponController playerWeaponController;
    private PlayerVisualController playerVisualController;
    private PlayerHealthController playerHealthController;

    // �÷��̾� �Է� ��ư ������
    public enum PlayerInputButtons
    {
        None,
        Jump,
        Shoot
    }

    // ��Ʈ��ũ ������Ʈ ���� �� �ʱ�ȭ
    public override void Spawned()
    {
        rigid = GetComponent<Rigidbody2D>();
        playerWeaponController = GetComponent<PlayerWeaponController>();
        playerVisualController = GetComponent<PlayerVisualController>();
        playerHealthController = GetComponent<PlayerHealthController>();

        SetLocalObjects();
        PlayerIsAlive = true;
    }

    // ���� ������Ʈ ����
    private void SetLocalObjects()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            // ���� �÷��̾��� ī�޶� ����
            cam.transform.SetParent(null);
            cam.SetActive(true);

            var nickName = GlobalManagers.Instance.NetworkRunnerController.LocalPlayerNickname;
            RpcSetNickName(nickName);
        }
        else
        {
            // ���Ͻ� �÷��̾��� ���� ����
            GetComponent<NetworkRigidbody2D>().InterpolationDataSource = InterpolationDataSources.Snapshots;
        }
    }

    // �г��� ���� RPC (Ŭ���̾�Ʈ -> ȣ��Ʈ)
    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcSetNickName(NetworkString<_8> nickName)
    {
        playerName = nickName;
    }

    // �г��� ���� �� ȣ��Ǵ� �ݹ�
    private static void OnNicknameChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.SetPlayerNickname(changed.Behaviour.playerName);
    }

    // �÷��̾� �г��� UI ������Ʈ
    private void SetPlayerNickname(NetworkString<_8> nickName)
    {
        playerNameText.text = nickName + " " + Object.InputAuthority.PlayerId;
    }

    // �÷��̾� ��� ó��
    public void KillPlayer()
    {
        const int RESPAWN_AMOUNT = 5;
        if (Runner.IsServer)
        {
            serverNextSpawnPoint = GlobalManagers.Instance.PlayerSpawnerController.GetRandomSpawnPoint();
            respawnToNewPointTimer = TickTimer.CreateFromSeconds(Runner, RESPAWN_AMOUNT - 1);
        }

        PlayerIsAlive = false;
        rigid.simulated = false;
        playerVisualController.TriggerDieAnimation();
        RespawnTimer = TickTimer.CreateFromSeconds(Runner, RESPAWN_AMOUNT);
    }

    // Fusion ������Ʈ �� ����
    public void BeforeUpdate()
    {
        if (Utils.IsLocalPlayer(Object) && AcceptAnyInput)
        {
            const string HORIZONTAL = "Horizontal";
            horizontal = Input.GetAxisRaw(HORIZONTAL);
        }
    }

    // ��Ʈ��ũ ���� ������Ʈ
    public override void FixedUpdateNetwork()
    {
        CheckRespawnTimer();

        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            if (AcceptAnyInput)
            {
                // �̵� ó��
                rigid.velocity = new Vector2(input.HorizontalInput * moveSpeed, rigid.velocity.y);
                CheckJumpInput(input);
                buttonsPrev = input.NetworkButtons;
            }
            else
            {
                rigid.velocity = Vector2.zero;
            }
        }

        playerVisualController.UpdateScaleTransforms(rigid.velocity);
    }

    // ������ Ÿ�̸� üũ
    private void CheckRespawnTimer()
    {
        if (PlayerIsAlive) return;

        // ���������� ����
        if (respawnToNewPointTimer.Expired(Runner))
        {
            GetComponent<NetworkRigidbody2D>().TeleportToPosition(serverNextSpawnPoint);
            respawnToNewPointTimer = TickTimer.None;
        }

        if (RespawnTimer.Expired(Runner))
        {
            RespawnTimer = TickTimer.None;
            RespawnPlayer();
        }
    }

    // �÷��̾� ������
    private void RespawnPlayer()
    {
        PlayerIsAlive = true;
        rigid.simulated = true;
        playerVisualController.TriggerRespawnAnimation();
        playerHealthController.ResetHealthAmountToMax();
    }

    // ������ ������Ʈ
    public override void Render()
    {
        playerVisualController.RendererVisuals(rigid.velocity, playerWeaponController.IsHoldingShootingKey);
    }

    // ���� �Է� üũ �� ó��
    private void CheckJumpInput(PlayerData input)
    {
        var transform1 = groundDetectionObj.transform;
        isGrounded = (bool)Runner.GetPhysicsScene2D().OverlapBox(transform1.position,
            transform1.localScale, 0, groundLayer);

        if (isGrounded)
        {
            var pressed = input.NetworkButtons.GetPressed(buttonsPrev);
            if (pressed.WasPressed(buttonsPrev, PlayerInputButtons.Jump))
            {
                rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
            }
        }
    }

    // ��Ʈ��ũ ������Ʈ ���� ��
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        GlobalManagers.Instance.ObjectPoolingManager.RemoveNetworkObjectFromDic(Object);
        Destroy(gameObject);
    }

    // �÷��̾� ��Ʈ��ũ �Է� ������ ����
    public PlayerData GetPlayerNetworkInput()
    {
        PlayerData data = new PlayerData();
        data.HorizontalInput = horizontal;
        data.GunPivotRotation = playerWeaponController.LocalQuaternionPivotRot;
        data.NetworkButtons.Set(PlayerInputButtons.Jump, Input.GetKey(KeyCode.Space));
        data.NetworkButtons.Set(PlayerInputButtons.Shoot, Input.GetButton("Fire1"));
        return data;
    }
}