using System;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    // 네트워크 프리팹과 스폰 포인트 
    [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;  // 플레이어 네트워크 프리팹
    [SerializeField] private Transform[] spawnPoints;  // 스폰 가능 위치들

    // 글로벌 매니저에 등록
    private void Awake()
    {
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.PlayerSpawnerController = this;
        }
    }

    // 네트워크 오브젝트 생성 시
    public override void Spawned()
    {
        // 서버에서만 실행
        if (Runner.IsServer)
        {
            // 활성화된 모든 플레이어 스폰
            foreach (var item in Runner.ActivePlayers)
            {
                SpawnPlayer(item);
            }
        }
    }

    // 랜덤한 스폰 포인트 반환
    public Vector2 GetRandomSpawnPoint()
    {
        var index = Random.Range(0, spawnPoints.Length - 1);
        return spawnPoints[index].position;
    }

    // 플레이어 스폰 처리
    private void SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            // 플레이어 ID에 따른 스폰 위치 결정
            var index = playerRef % spawnPoints.Length;
            var spawnPoint = spawnPoints[index].transform.position;

            // 플레이어 네트워크 오브젝트 생성
            var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);

            // 생성된 오브젝트를 해당 플레이어에 할당
            Runner.SetPlayerObject(playerRef, playerObject);
        }
    }

    // 플레이어 디스폰 처리
    private void DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            // 플레이어 오브젝트 찾아서 제거
            if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
            {
                Runner.Despawn(playerNetworkObject);
            }

            // 플레이어 오브젝트 초기화
            Runner.SetPlayerObject(playerRef, null);
        }
    }

    // 플레이어 입장 시 호출
    public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

    // 플레이어 퇴장 시 호출
    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }
}