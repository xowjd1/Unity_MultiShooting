using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    // 필요한 컴포넌트 참조
    [SerializeField] private Animator animator;           // 플레이어 애니메이터
    [SerializeField] private Transform pivotGunTr;       // 무기 피봇 트랜스폼
    [SerializeField] private Transform canvasTr;         // UI 캔버스 트랜스폼

    // 애니메이터 파라미터 해시값
    private readonly int isMovingHash = Animator.StringToHash("IsWalking");    // 걷기 상태
    private readonly int isShootingHash = Animator.StringToHash("IsShooting"); // 발사 상태

    // 캐릭터 방향 및 초기화 관련 변수
    private bool isFacingRight = true;  // 오른쪽 바라보는지 여부
    private bool init;                  // 초기화 완료 여부

    // 원본 스케일 값 저장
    private Vector3 originalPlayerScale;     // 플레이어 원본 스케일
    private Vector3 originalCanvasScale;     // 캔버스 원본 스케일
    private Vector3 originalGunPivotScale;   // 무기 피봇 원본 스케일

    // 초기 설정
    private void Start()
    {
        // 원본 스케일 값 저장
        originalPlayerScale = this.transform.localScale;
        originalCanvasScale = canvasTr.transform.localScale;
        originalGunPivotScale = pivotGunTr.transform.localScale;

        // 발사 애니메이션 레이어 가중치 설정
        const int SHOOTING_LAYER_INDEX = 1;
        animator.SetLayerWeight(SHOOTING_LAYER_INDEX, 1);

        init = true;
    }

    // 사망 애니메이션 실행
    public void TriggerDieAnimation()
    {
        const string TRIGGER = "Die";
        animator.SetTrigger(TRIGGER);
    }

    // 리스폰 애니메이션 실행
    public void TriggerRespawnAnimation()
    {
        const string TRIGGER = "Respawn";
        animator.SetTrigger(TRIGGER);
    }

    // 시각적 효과 업데이트 (이동, 발사)
    public void RendererVisuals(Vector2 velocity, bool isShooting)
    {
        if (!init) return;

        // 이동 상태 체크 및 애니메이션 설정
        var isMoving = velocity.x > 0.1f || velocity.x < -0.1f;
        animator.SetBool(isMovingHash, isMoving);
        animator.SetBool(isShootingHash, isShooting);
    }

    // 캐릭터 방향에 따른 스케일 업데이트
    public void UpdateScaleTransforms(Vector2 velocity)
    {
        if (!init) return;

        // 이동 방향에 따라 바라보는 방향 설정
        if (velocity.x > 0.1f)
        {
            isFacingRight = true;
        }
        else if (velocity.x < -0.1f)
        {
            isFacingRight = false;
        }

        // 각 오브젝트의 스케일 업데이트
        SetObjectLocalScaleBasedOnDir(gameObject, originalPlayerScale);
        SetObjectLocalScaleBasedOnDir(canvasTr.gameObject, originalCanvasScale);
        SetObjectLocalScaleBasedOnDir(pivotGunTr.gameObject, originalGunPivotScale);
    }

    // 오브젝트 방향에 따른 스케일 설정
    private void SetObjectLocalScaleBasedOnDir(GameObject obj, Vector3 originalScale)
    {
        var yValue = originalScale.y;
        var zValue = originalScale.z;
        var xValue = isFacingRight ? originalScale.x : -originalScale.x;
        obj.transform.localScale = new Vector3(xValue, yValue, zValue);
    }
}