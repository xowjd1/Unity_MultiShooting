using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : NetworkBehaviour
{
    [SerializeField] private LayerMask deathGroundLayerMask;
    [SerializeField] private Animator bloodScreenHitAnimator;
    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private Image fillAmountImg;
    [SerializeField] private TextMeshProUGUI healthAmountText;

    [Networked(OnChanged = nameof(HealthAmountChanged))] private int currentHealthAmount { get; set; }

    private const int MAX_HEALTH_AMOUNT = 100;
    private PlayerController playerController;
    private Collider2D coll;
    
    public override void Spawned()
    {
        coll = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>();
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }

    public override void FixedUpdateNetwork()
    {
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ReducePlayerHealth(int damage)
    {
        currentHealthAmount -= damage;
    }

    private static void HealthAmountChanged(Changed<PlayerHealthController> changed)
    {
        var currentHealth = changed.Behaviour.currentHealthAmount;
        
        changed.LoadOld();
        var oldHealthAmount = changed.Behaviour.currentHealthAmount;

        //Only if the current health is not the same as the prev one
        if (currentHealth != oldHealthAmount)
        {
            changed.Behaviour.UpdateVisuals(currentHealth);

            //We did not respawn or just spawned
            if (currentHealth != MAX_HEALTH_AMOUNT)
            {
                changed.Behaviour.PlayerGotHit(currentHealth);
            }
        }
    }

    private void UpdateVisuals(int healthAmount)
    {
        var num = (float)healthAmount / MAX_HEALTH_AMOUNT;
        fillAmountImg.fillAmount = num;
        healthAmountText.text = $"{healthAmount}/{MAX_HEALTH_AMOUNT}";
    }


    private void PlayerGotHit(int healthAmount)
    {
        if (Utils.IsLocalPlayer(Object))
        {
            Debug.Log("LOCAL PLAYER GOT HIT!");

            const string BLOOD_HIT_CLIP_NAME = "BloodScreenHit";
            bloodScreenHitAnimator.Play(BLOOD_HIT_CLIP_NAME);

            var shakeAmount = new Vector3(0.2f, 0.1f);
            playerCameraController.ShakeCamera(shakeAmount);
        }

        if (healthAmount <= 0)
        {
            playerController.KillPlayer();
            Debug.Log("Player is DEAD!");
        }
    }


    public void ResetHealthAmountToMax()
    {
        currentHealthAmount = MAX_HEALTH_AMOUNT; 
    }
    
    
    
}
