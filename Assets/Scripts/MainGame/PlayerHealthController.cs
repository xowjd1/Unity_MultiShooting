using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : NetworkBehaviour
{
    // 필요한 레이어와 컴포넌트 참조
    [SerializeField] private LayerMask deathGroundLayerMask;    // 데스존 레이어
    [SerializeField] private Animator bloodScreenHitAnimator;    // 피격 효과 애니메이터
    [SerializeField] private PlayerCameraController playerCameraController;  // 카메라 컨트롤러
    [SerializeField] private Image fillAmountImg;               // 체력바 이미지
    [SerializeField] private TextMeshProUGUI healthAmountText;  // 체력 수치 텍스트

    // 네트워크 동기화되는 현재 체력
    [Networked(OnChanged = nameof(HealthAmountChanged))]
    private int currentHealthAmount { get; set; }

    private const int MAX_HEALTH_AMOUNT = 100;  // 최대 체력 상수
    private PlayerController playerController;
    private Collider2D coll;

    // 초기화
    public override void Spawned()
    {
        coll = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>();
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }

    // 네트워크 물리 업데이트
    public override void FixedUpdateNetwork()
    {
        // 서버에서만 데스존 체크
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

    // 서버에 체력 감소 요청
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ReducePlayerHealth(int damage)
    {
        currentHealthAmount -= damage;
    }

    // 체력 변경 시 호출되는 콜백
    private static void HealthAmountChanged(Changed<PlayerHealthController> changed)
    {
        var currentHealth = changed.Behaviour.currentHealthAmount;

        changed.LoadOld();
        var oldHealthAmount = changed.Behaviour.currentHealthAmount;

        // 체력이 변경된 경우에만 처리
        if (currentHealth != oldHealthAmount)
        {
            changed.Behaviour.UpdateVisuals(currentHealth);
            // 리스폰이나 초기 스폰이 아닌 경우
            if (currentHealth != MAX_HEALTH_AMOUNT)
            {
                changed.Behaviour.PlayerGotHit(currentHealth);
            }
        }
    }

    // UI 업데이트
    private void UpdateVisuals(int healthAmount)
    {
        var num = (float)healthAmount / MAX_HEALTH_AMOUNT;
        fillAmountImg.fillAmount = num;
        healthAmountText.text = $"{healthAmount}/{MAX_HEALTH_AMOUNT}";
    }

    // 피격 처리
    private void PlayerGotHit(int healthAmount)
    {
        // 로컬 플레이어 피격 효과
        if (Utils.IsLocalPlayer(Object))
        {
            Debug.Log("LOCAL PLAYER GOT HIT!");
            const string BLOOD_HIT_CLIP_NAME = "BloodScreenHit";
            bloodScreenHitAnimator.Play(BLOOD_HIT_CLIP_NAME);
            var shakeAmount = new Vector3(0.2f, 0.1f);
            playerCameraController.ShakeCamera(shakeAmount);
        }

        // 체력이 0 이하면 사망 처리
        if (healthAmount <= 0)
        {
            playerController.KillPlayer();
            Debug.Log("Player is DEAD!");
        }
    }

    // 체력 최대치로 복구
    public void ResetHealthAmountToMax()
    {
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }
}