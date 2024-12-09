using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    // �ʿ��� ������Ʈ ����
    [SerializeField] private Animator animator;           // �÷��̾� �ִϸ�����
    [SerializeField] private Transform pivotGunTr;       // ���� �Ǻ� Ʈ������
    [SerializeField] private Transform canvasTr;         // UI ĵ���� Ʈ������

    // �ִϸ����� �Ķ���� �ؽð�
    private readonly int isMovingHash = Animator.StringToHash("IsWalking");    // �ȱ� ����
    private readonly int isShootingHash = Animator.StringToHash("IsShooting"); // �߻� ����

    // ĳ���� ���� �� �ʱ�ȭ ���� ����
    private bool isFacingRight = true;  // ������ �ٶ󺸴��� ����
    private bool init;                  // �ʱ�ȭ �Ϸ� ����

    // ���� ������ �� ����
    private Vector3 originalPlayerScale;     // �÷��̾� ���� ������
    private Vector3 originalCanvasScale;     // ĵ���� ���� ������
    private Vector3 originalGunPivotScale;   // ���� �Ǻ� ���� ������

    // �ʱ� ����
    private void Start()
    {
        // ���� ������ �� ����
        originalPlayerScale = this.transform.localScale;
        originalCanvasScale = canvasTr.transform.localScale;
        originalGunPivotScale = pivotGunTr.transform.localScale;

        // �߻� �ִϸ��̼� ���̾� ����ġ ����
        const int SHOOTING_LAYER_INDEX = 1;
        animator.SetLayerWeight(SHOOTING_LAYER_INDEX, 1);

        init = true;
    }

    // ��� �ִϸ��̼� ����
    public void TriggerDieAnimation()
    {
        const string TRIGGER = "Die";
        animator.SetTrigger(TRIGGER);
    }

    // ������ �ִϸ��̼� ����
    public void TriggerRespawnAnimation()
    {
        const string TRIGGER = "Respawn";
        animator.SetTrigger(TRIGGER);
    }

    // �ð��� ȿ�� ������Ʈ (�̵�, �߻�)
    public void RendererVisuals(Vector2 velocity, bool isShooting)
    {
        if (!init) return;

        // �̵� ���� üũ �� �ִϸ��̼� ����
        var isMoving = velocity.x > 0.1f || velocity.x < -0.1f;
        animator.SetBool(isMovingHash, isMoving);
        animator.SetBool(isShootingHash, isShooting);
    }

    // ĳ���� ���⿡ ���� ������ ������Ʈ
    public void UpdateScaleTransforms(Vector2 velocity)
    {
        if (!init) return;

        // �̵� ���⿡ ���� �ٶ󺸴� ���� ����
        if (velocity.x > 0.1f)
        {
            isFacingRight = true;
        }
        else if (velocity.x < -0.1f)
        {
            isFacingRight = false;
        }

        // �� ������Ʈ�� ������ ������Ʈ
        SetObjectLocalScaleBasedOnDir(gameObject, originalPlayerScale);
        SetObjectLocalScaleBasedOnDir(canvasTr.gameObject, originalCanvasScale);
        SetObjectLocalScaleBasedOnDir(pivotGunTr.gameObject, originalGunPivotScale);
    }

    // ������Ʈ ���⿡ ���� ������ ����
    private void SetObjectLocalScaleBasedOnDir(GameObject obj, Vector3 originalScale)
    {
        var yValue = originalScale.y;
        var zValue = originalScale.z;
        var xValue = isFacingRight ? originalScale.x : -originalScale.x;
        obj.transform.localScale = new Vector3(xValue, yValue, zValue);
    }
}