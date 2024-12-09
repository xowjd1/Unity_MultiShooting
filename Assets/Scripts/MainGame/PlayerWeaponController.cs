using Fusion;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
{
    // 무기 회전 관련 속성
    public Quaternion LocalQuaternionPivotRot { get; private set; }

    // 컴포넌트 및 설정 참조
    [SerializeField] private NetworkPrefabRef bulletPrefab = NetworkPrefabRef.Empty;  // 총알 프리팹
    [SerializeField] private Transform firePointPos;         // 발사 위치
    [SerializeField] private float delayBetweenShots = 0.18f;  // 발사 간격
    [SerializeField] private ParticleSystem muzzleEffect;    // 총구 이펙트
    [SerializeField] private Camera localCam;                // 로컬 카메라
    [SerializeField] private Transform pivotToRotate;        // 회전할 무기 피봇

    // 네트워크 동기화 변수
    [Networked, HideInInspector] public NetworkBool IsHoldingShootingKey { get; private set; }
    [Networked(OnChanged = nameof(OnMuzzleEffectStateChanged))] private NetworkBool playMuzzleEffect { get; set; }
    [Networked] private Quaternion currentPlayerPivotRotation { get; set; }
    [Networked] private NetworkButtons buttonsPrev { get; set; }
    [Networked] private TickTimer shootCoolDown { get; set; }

    private PlayerController playerController;

    // 초기화
    public override void Spawned()
    {
        playerController = GetComponent<PlayerController>();
    }

    // 마우스 위치 기반 무기 회전 계산
    public void BeforeUpdate()
    {
        if (Utils.IsLocalPlayer(Object) && playerController.AcceptAnyInput)
        {
            var direction = localCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            LocalQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // 네트워크 동기화된 물리 업데이트
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
                // 입력 불가 상태일 때 초기화
                IsHoldingShootingKey = false;
                playMuzzleEffect = false;
                buttonsPrev = default;
            }
        }
        pivotToRotate.rotation = currentPlayerPivotRotation;
    }

    // 발사 입력 체크 및 처리
    private void CheckShootInput(PlayerData input)
    {
        var currentBtns = input.NetworkButtons.GetPressed(buttonsPrev);
        IsHoldingShootingKey = currentBtns.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot);

        // 발사 쿨다운 체크 및 총알 생성
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

    // 총구 이펙트 상태 변경 콜백
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

    // 총구 이펙트 재생/정지
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