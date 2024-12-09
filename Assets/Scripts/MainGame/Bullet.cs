using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    // 레이어 마스크 설정 
    [SerializeField] private LayerMask playerLayerMask;    // 플레이어 감지용 레이어
    [SerializeField] private LayerMask groundLayerMask;    // 지형 감지용 레이어

    // 총알 기본 속성
    [SerializeField] private int bulletDmg = 10;           // 총알 데미지
    [SerializeField] private float moveSpeed = 20;         // 총알 이동 속도
    [SerializeField] private float lifeTimeAmount = 0.8f;  // 총알 생존 시간

    // 네트워크 동기화 변수
    [Networked] private NetworkBool didHitSomething { get; set; }  // 충돌 여부
    [Networked] private TickTimer lifeTimeTimer { get; set; }      // 생존 시간 타이머

    private Collider2D coll;

    // 총알 생성 시 초기화
    public override void Spawned()
    {
        coll = GetComponent<Collider2D>();
        lifeTimeTimer = TickTimer.CreateFromSeconds(Runner, lifeTimeAmount);
    }

    // 네트워크 동기화된 물리 업데이트
    public override void FixedUpdateNetwork()
    {
        // 충돌하지 않은 경우에만 충돌 체크 수행
        if (!didHitSomething)
        {
            CheckIfHitGround();      // 지형 충돌 체크
            CheckIfWeHitAPlayer();   // 플레이어 충돌 체크
        }

        // 생존 시간이 남아있고 충돌하지 않은 경우 이동
        if (lifeTimeTimer.ExpiredOrNotRunning(Runner) == false && !didHitSomething)
        {
            transform.Translate(transform.right * moveSpeed * Runner.DeltaTime, Space.World);
        }

        // 생존 시간 종료나 충돌 시 총알 제거
        if (lifeTimeTimer.Expired(Runner) || didHitSomething)
        {
            lifeTimeTimer = TickTimer.None;
            Runner.Despawn(Object);
        }
    }

    // 지형 충돌 체크
    private void CheckIfHitGround()
    {
        var groundCollider = Runner.GetPhysicsScene2D()
            .OverlapBox(transform.position, coll.bounds.size, 0, groundLayerMask);
        if (groundCollider != default)
        {
            didHitSomething = true;
        }
    }

    // 렉 보정을 위한 히트 리스트
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    // 플레이어 충돌 체크
    private void CheckIfWeHitAPlayer()
    {
        // 렉 보정을 적용한 충돌 체크
        Runner.LagCompensation.OverlapBox(transform.position, coll.bounds.size, Quaternion.identity,
            Object.InputAuthority, hits, playerLayerMask);

        if (hits.Count > 0)
        {
            foreach (var item in hits)
            {
                if (item.Hitbox != null)
                {
                    var player = item.Hitbox.GetComponentInParent<PlayerController>();
                    // 자신의 총알에 맞지 않도록 체크
                    var didNotHitOurOwnPlayer = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;

                    // 다른 플레이어이고 살아있는 경우에만 데미지 처리
                    if (didNotHitOurOwnPlayer && player.PlayerIsAlive)
                    {
                        // 서버에서만 데미지 처리
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