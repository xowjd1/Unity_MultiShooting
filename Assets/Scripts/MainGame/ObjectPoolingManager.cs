using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour, INetworkObjectPool
{
    // ��ųʸ�: ������(Ű)�� ������ ��Ʈ��ũ ������Ʈ ����Ʈ(��)�� ����
    // ��: �Ѿ� �������� Ű�� �ǰ�, ������ �Ѿ� ������Ʈ���� ����Ʈ�� �����
    private Dictionary<NetworkObject, List<NetworkObject>> prefabsThatHadBeenInstantiated = new();

    private void Start()
    {
        // �۷ι� �Ŵ����� ������Ʈ Ǯ�� �Ŵ��� ���
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.ObjectPoolingManager = this;
        }
    }

    // Runner.Spawn�� ȣ��� �� ����Ǵ� �Լ�
    public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
    {
        NetworkObject networkObject = null;
        NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out var prefab);
        prefabsThatHadBeenInstantiated.TryGetValue(prefab, out var networkObjects);
        bool foundMatch = false;

        // ��Ȱ��ȭ�� ������Ʈ�� �ִ��� �˻�
        if (networkObjects?.Count > 0)
        {
            foreach (var item in networkObjects)
            {
                if (item != null && item.gameObject.activeSelf == false)
                {
                    // ���� ������ ������Ʈ �߰�
                    networkObject = item;
                    foundMatch = true;
                    break;
                }
            }
        }

        // ���� ������ ������Ʈ�� ���ų� ���ο� �������� ��� ���� ����
        if (foundMatch == false)
        {
            networkObject = CreateObjectInstance(prefab);
        }

        return networkObject;
    }

    // ���ο� ��Ʈ��ũ ������Ʈ ����
    private NetworkObject CreateObjectInstance(NetworkObject prefab)
    {
        var obj = Instantiate(prefab);

        // ��ųʸ��� �߰�
        if (prefabsThatHadBeenInstantiated.TryGetValue(prefab, out var instanceData))
        {
            instanceData.Add(obj);
        }
        else
        {
            var list = new List<NetworkObject> { obj };
            prefabsThatHadBeenInstantiated.Add(prefab, list);
        }
        return obj;
    }

    // Runner.Despawn�� ȣ��� �� ����Ǵ� �Լ�
    public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
    {
        instance.gameObject.SetActive(false);
    }

    // ��ųʸ����� ��Ʈ��ũ ������Ʈ ����
    public void RemoveNetworkObjectFromDic(NetworkObject obj)
    {
        if (prefabsThatHadBeenInstantiated.Count > 0)
        {
            foreach (var item in prefabsThatHadBeenInstantiated)
            {
                foreach (var networkObject in item.Value.Where(networkObject => networkObject == obj))
                {
                    item.Value.Remove(networkObject);
                    break;
                }
            }
        }
    }
}