using System;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    // ��Ʈ��ũ �����հ� ���� ����Ʈ 
    [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;  // �÷��̾� ��Ʈ��ũ ������
    [SerializeField] private Transform[] spawnPoints;  // ���� ���� ��ġ��

    // �۷ι� �Ŵ����� ���
    private void Awake()
    {
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.PlayerSpawnerController = this;
        }
    }

    // ��Ʈ��ũ ������Ʈ ���� ��
    public override void Spawned()
    {
        // ���������� ����
        if (Runner.IsServer)
        {
            // Ȱ��ȭ�� ��� �÷��̾� ����
            foreach (var item in Runner.ActivePlayers)
            {
                SpawnPlayer(item);
            }
        }
    }

    // ������ ���� ����Ʈ ��ȯ
    public Vector2 GetRandomSpawnPoint()
    {
        var index = Random.Range(0, spawnPoints.Length - 1);
        return spawnPoints[index].position;
    }

    // �÷��̾� ���� ó��
    private void SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            // �÷��̾� ID�� ���� ���� ��ġ ����
            var index = playerRef % spawnPoints.Length;
            var spawnPoint = spawnPoints[index].transform.position;

            // �÷��̾� ��Ʈ��ũ ������Ʈ ����
            var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);

            // ������ ������Ʈ�� �ش� �÷��̾ �Ҵ�
            Runner.SetPlayerObject(playerRef, playerObject);
        }
    }

    // �÷��̾� ���� ó��
    private void DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            // �÷��̾� ������Ʈ ã�Ƽ� ����
            if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
            {
                Runner.Despawn(playerNetworkObject);
            }

            // �÷��̾� ������Ʈ �ʱ�ȭ
            Runner.SetPlayerObject(playerRef, null);
        }
    }

    // �÷��̾� ���� �� ȣ��
    public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

    // �÷��̾� ���� �� ȣ��
    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }
}