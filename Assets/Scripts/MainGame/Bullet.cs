using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    // ���̾� ����ũ ���� 
    [SerializeField] private LayerMask playerLayerMask;    // �÷��̾� ������ ���̾�
    [SerializeField] private LayerMask groundLayerMask;    // ���� ������ ���̾�

    // �Ѿ� �⺻ �Ӽ�
    [SerializeField] private int bulletDmg = 10;           // �Ѿ� ������
    [SerializeField] private float moveSpeed = 20;         // �Ѿ� �̵� �ӵ�
    [SerializeField] private float lifeTimeAmount = 0.8f;  // �Ѿ� ���� �ð�

    // ��Ʈ��ũ ����ȭ ����
    [Networked] private NetworkBool didHitSomething { get; set; }  // �浹 ����
    [Networked] private TickTimer lifeTimeTimer { get; set; }      // ���� �ð� Ÿ�̸�

    private Collider2D coll;

    // �Ѿ� ���� �� �ʱ�ȭ
    public override void Spawned()
    {
        coll = GetComponent<Collider2D>();
        lifeTimeTimer = TickTimer.CreateFromSeconds(Runner, lifeTimeAmount);
    }

    // ��Ʈ��ũ ����ȭ�� ���� ������Ʈ
    public override void FixedUpdateNetwork()
    {
        // �浹���� ���� ��쿡�� �浹 üũ ����
        if (!didHitSomething)
        {
            CheckIfHitGround();      // ���� �浹 üũ
            CheckIfWeHitAPlayer();   // �÷��̾� �浹 üũ
        }

        // ���� �ð��� �����ְ� �浹���� ���� ��� �̵�
        if (lifeTimeTimer.ExpiredOrNotRunning(Runner) == false && !didHitSomething)
        {
            transform.Translate(transform.right * moveSpeed * Runner.DeltaTime, Space.World);
        }

        // ���� �ð� ���ᳪ �浹 �� �Ѿ� ����
        if (lifeTimeTimer.Expired(Runner) || didHitSomething)
        {
            lifeTimeTimer = TickTimer.None;
            Runner.Despawn(Object);
        }
    }

    // ���� �浹 üũ
    private void CheckIfHitGround()
    {
        var groundCollider = Runner.GetPhysicsScene2D()
            .OverlapBox(transform.position, coll.bounds.size, 0, groundLayerMask);
        if (groundCollider != default)
        {
            didHitSomething = true;
        }
    }

    // �� ������ ���� ��Ʈ ����Ʈ
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    // �÷��̾� �浹 üũ
    private void CheckIfWeHitAPlayer()
    {
        // �� ������ ������ �浹 üũ
        Runner.LagCompensation.OverlapBox(transform.position, coll.bounds.size, Quaternion.identity,
            Object.InputAuthority, hits, playerLayerMask);

        if (hits.Count > 0)
        {
            foreach (var item in hits)
            {
                if (item.Hitbox != null)
                {
                    var player = item.Hitbox.GetComponentInParent<PlayerController>();
                    // �ڽ��� �Ѿ˿� ���� �ʵ��� üũ
                    var didNotHitOurOwnPlayer = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;

                    // �ٸ� �÷��̾��̰� ����ִ� ��쿡�� ������ ó��
                    if (didNotHitOurOwnPlayer && player.PlayerIsAlive)
                    {
                        // ���������� ������ ó��
                        if (Runner.IsServer)
                        {
                            player.GetComponent<PlayerHealthController>().Rpc_ReducePlayerHealth(bulletDmg);
                        }

                        didHitSomething = true;
                        break;
                    }
                }
            }
        }
    }
}