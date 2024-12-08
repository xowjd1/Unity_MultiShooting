using Fusion;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
{
    public Quaternion LocalQuaternionPivotRot { get; private set; }
    [SerializeField] private NetworkPrefabRef bulletPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private Transform firePointPos;
    [SerializeField] private float delayBetweenShots = 0.18f;
    [SerializeField] private ParticleSystem muzzleEffect;
    [SerializeField] private Camera localCam;
    [SerializeField] private Transform pivotToRotate;

    [Networked, HideInInspector] public NetworkBool IsHoldingShootingKey { get; private set; }
    [Networked(OnChanged = nameof(OnMuzzleEffectStateChanged))] private NetworkBool playMuzzleEffect { get; set; }
    [Networked] private Quaternion currentPlayerPivotRotation { get; set; }
    
    [Networked] private NetworkButtons buttonsPrev { get; set; }
    [Networked] private TickTimer shootCoolDown { get; set; }

    private PlayerController playerController;

    public override void Spawned()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void BeforeUpdate()
    {
        if (Utils.IsLocalPlayer(Object) && playerController.AcceptAnyInput)
        {
            var direction = localCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            LocalQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

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
                IsHoldingShootingKey = false;
                playMuzzleEffect = false;
                buttonsPrev = default;
            }
        }

        pivotToRotate.rotation = currentPlayerPivotRotation;
    }

    private void CheckShootInput(PlayerData input)
    {
        var currentBtns = input.NetworkButtons.GetPressed(buttonsPrev);

        IsHoldingShootingKey = currentBtns.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot);
        
        if (currentBtns.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot) && shootCoolDown.ExpiredOrNotRunning(Runner))
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















