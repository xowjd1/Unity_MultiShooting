using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : NetworkBehaviour
{
    // �ʿ��� ���̾�� ������Ʈ ����
    [SerializeField] private LayerMask deathGroundLayerMask;    // ������ ���̾�
    [SerializeField] private Animator bloodScreenHitAnimator;    // �ǰ� ȿ�� �ִϸ�����
    [SerializeField] private PlayerCameraController playerCameraController;  // ī�޶� ��Ʈ�ѷ�
    [SerializeField] private Image fillAmountImg;               // ü�¹� �̹���
    [SerializeField] private TextMeshProUGUI healthAmountText;  // ü�� ��ġ �ؽ�Ʈ

    // ��Ʈ��ũ ����ȭ�Ǵ� ���� ü��
    [Networked(OnChanged = nameof(HealthAmountChanged))]
    private int currentHealthAmount { get; set; }

    private const int MAX_HEALTH_AMOUNT = 100;  // �ִ� ü�� ���
    private PlayerController playerController;
    private Collider2D coll;

    // �ʱ�ȭ
    public override void Spawned()
    {
        coll = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>();
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }

    // ��Ʈ��ũ ���� ������Ʈ
    public override void FixedUpdateNetwork()
    {
        // ���������� ������ üũ
        if (Runner.IsServer && playerController.PlayerIsAlive)
        {
            var didHitCollider = Runner.GetPhysicsScene2D()
                .OverlapBox(transform.position, coll.bounds.size, 0, deathGroundLayerMask);
            if (didHitCollider != default)
            {
                Rpc_ReducePlayerHealth(MAX_HEALTH_AMOUNT);
            }
        }
    }

    // ������ ü�� ���� ��û
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ReducePlayerHealth(int damage)
    {
        currentHealthAmount -= damage;
    }

    // ü�� ���� �� ȣ��Ǵ� �ݹ�
    private static void HealthAmountChanged(Changed<PlayerHealthController> changed)
    {
        var currentHealth = changed.Behaviour.currentHealthAmount;

        changed.LoadOld();
        var oldHealthAmount = changed.Behaviour.currentHealthAmount;

        // ü���� ����� ��쿡�� ó��
        if (currentHealth != oldHealthAmount)
        {
            changed.Behaviour.UpdateVisuals(currentHealth);
            // �������̳� �ʱ� ������ �ƴ� ���
            if (currentHealth != MAX_HEALTH_AMOUNT)
            {
                changed.Behaviour.PlayerGotHit(currentHealth);
            }
        }
    }

    // UI ������Ʈ
    private void UpdateVisuals(int healthAmount)
    {
        var num = (float)healthAmount / MAX_HEALTH_AMOUNT;
        fillAmountImg.fillAmount = num;
        healthAmountText.text = $"{healthAmount}/{MAX_HEALTH_AMOUNT}";
    }

    // �ǰ� ó��
    private void PlayerGotHit(int healthAmount)
    {
        // ���� �÷��̾� �ǰ� ȿ��
        if (Utils.IsLocalPlayer(Object))
        {
            Debug.Log("LOCAL PLAYER GOT HIT!");
            const string BLOOD_HIT_CLIP_NAME = "BloodScreenHit";
            bloodScreenHitAnimator.Play(BLOOD_HIT_CLIP_NAME);
            var shakeAmount = new Vector3(0.2f, 0.1f);
            playerCameraController.ShakeCamera(shakeAmount);
        }

        // ü���� 0 ���ϸ� ��� ó��
        if (healthAmount <= 0)
        {
            playerController.KillPlayer();
            Debug.Log("Player is DEAD!");
        }
    }

    // ü�� �ִ�ġ�� ����
    public void ResetHealthAmountToMax()
    {
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }
}