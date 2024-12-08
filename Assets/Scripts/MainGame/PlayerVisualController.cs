using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pivotGunTr;
    [SerializeField] private Transform canvasTr;

    private readonly int isMovingHash = Animator.StringToHash("IsWalking");
    private readonly int isShootingHash = Animator.StringToHash("IsShooting");
    private bool isFacingRight = true;
    private bool init;
    private Vector3 originalPlayerScale;
    private Vector3 originalCanvasScale;
    private Vector3 originalGunPivotScale;

    private void Start()
    {
        originalPlayerScale = this.transform.localScale;
        originalCanvasScale = canvasTr.transform.localScale;
        originalGunPivotScale = pivotGunTr.transform.localScale;

        const int SHOOTING_LAYER_INDEX = 1;
        animator.SetLayerWeight(SHOOTING_LAYER_INDEX, 1);
        
        init = true;
    }

    public void TriggerDieAnimation()
    {
        const string TRIGGER = "Die";
        animator.SetTrigger(TRIGGER);
    }
    
    public void TriggerRespawnAnimation()
    {
        const string TRIGGER = "Respawn";
        animator.SetTrigger(TRIGGER);
    }
    
    public void RendererVisuals(Vector2 velocity, bool isShooting)
    {
        if (!init) return;
        
        var isMoving = velocity.x > 0.1f || velocity.x < -0.1f;
        animator.SetBool(isMovingHash, isMoving);
        animator.SetBool(isShootingHash, isShooting);
    }

    public void UpdateScaleTransforms(Vector2 velocity)
    {
        if (!init) return;
        
        if (velocity.x > 0.1f)
        {
            isFacingRight = true;
        }
        else if (velocity.x < -0.1f)
        {
            isFacingRight = false;
        }

        SetObjectLocalScaleBasedOnDir(gameObject, originalPlayerScale);
        SetObjectLocalScaleBasedOnDir(canvasTr.gameObject, originalCanvasScale);
        SetObjectLocalScaleBasedOnDir(pivotGunTr.gameObject, originalGunPivotScale);
    }

    private void SetObjectLocalScaleBasedOnDir(GameObject obj, Vector3 originalScale)
    {
        var yValue = originalScale.y;
        var zValue = originalScale.z;
        var xValue = isFacingRight ? originalScale.x : -originalScale.x;
        obj.transform.localScale = new Vector3(xValue, yValue, zValue);
    }
}
