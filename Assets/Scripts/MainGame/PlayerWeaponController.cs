using Fusion;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
{
    // ���� ȸ�� ���� �Ӽ�
    public Quaternion LocalQuaternionPivotRot { get; private set; }

    // ������Ʈ �� ���� ����
    [SerializeField] private NetworkPrefabRef bulletPrefab = NetworkPrefabRef.Empty;  // �Ѿ� ������
    [SerializeField] private Transform firePointPos;         // �߻� ��ġ
    [SerializeField] private float delayBetweenShots = 0.18f;  // �߻� ����
    [SerializeField] private ParticleSystem muzzleEffect;    // �ѱ� ����Ʈ
    [SerializeField] private Camera localCam;                // ���� ī�޶�
    [SerializeField] private Transform pivotToRotate;        // ȸ���� ���� �Ǻ�

    // ��Ʈ��ũ ����ȭ ����
    [Networked, HideInInspector] public NetworkBool IsHoldingShootingKey { get; private set; }
    [Networked(OnChanged = nameof(OnMuzzleEffectStateChanged))] private NetworkBool playMuzzleEffect { get; set; }
    [Networked] private Quaternion currentPlayerPivotRotation { get; set; }
    [Networked] private NetworkButtons buttonsPrev { get; set; }
    [Networked] private TickTimer shootCoolDown { get; set; }

    private PlayerController playerController;

    // �ʱ�ȭ
    public override void Spawned()
    {
        playerController = GetComponent<PlayerController>();
    }

    // ���콺 ��ġ ��� ���� ȸ�� ���
    public void BeforeUpdate()
    {
        if (Utils.IsLocalPlayer(Object) && playerController.AcceptAnyInput)
        {
            var direction = localCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            LocalQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // ��Ʈ��ũ ����ȭ�� ���� ������Ʈ
    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            if (playerController.AcceptAnyInput)
            {
                CheckShootInput(input);
                currentPlayerPivotRotation = input.GunPivotRotation;
                buttonsPrev = input.NetworkButtons;
            }
            else
            {
                // �Է� �Ұ� ������ �� �ʱ�ȭ
                IsHoldingShootingKey = false;
                playMuzzleEffect = false;
                buttonsPrev = default;
            }
        }
        pivotToRotate.rotation = currentPlayerPivotRotation;
    }

    // �߻� �Է� üũ �� ó��
    private void CheckShootInput(PlayerData input)
    {
        var currentBtns = input.NetworkButtons.GetPressed(buttonsPrev);
        IsHoldingShootingKey = currentBtns.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot);

        // �߻� ��ٿ� üũ �� �Ѿ� ����
        if (currentBtns.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot) &&
            shootCoolDown.ExpiredOrNotRunning(Runner))
        {
            playMuzzleEffect = true;
            shootCoolDown = TickTimer.CreateFromSeconds(Runner, delayBetweenShots);
            Runner.Spawn(bulletPrefab, firePointPos.position, firePointPos.rotation, Object.InputAuthority);
        }
        else
        {
            playMuzzleEffect = false;
        }
    }

    // �ѱ� ����Ʈ ���� ���� �ݹ�
    private static void OnMuzzleEffectStateChanged(Changed<PlayerWeaponController> changed)
    {
        var currentState = changed.Behaviour.playMuzzleEffect;
        changed.LoadOld();
        var oldState = changed.Behaviour.playMuzzleEffect;
        if (oldState != currentState)
        {
            changed.Behaviour.PlayOrStopMuzzleEffect(currentState);
        }
    }

    // �ѱ� ����Ʈ ���/����
    private void PlayOrStopMuzzleEffect(bool play)
    {
        if (play)
        {
            muzzleEffect.Play();
        }
        else
        {
            muzzleEffect.Stop();
        }
    }
}