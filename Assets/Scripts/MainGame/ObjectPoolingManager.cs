using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour, INetworkObjectPool
{
    // 딕셔너리: 프리팹(키)과 생성된 네트워크 오브젝트 리스트(값)를 관리
    // 예: 총알 프리팹이 키가 되고, 생성된 총알 오브젝트들이 리스트에 저장됨
    private Dictionary<NetworkObject, List<NetworkObject>> prefabsThatHadBeenInstantiated = new();

    private void Start()
    {
        // 글로벌 매니저에 오브젝트 풀링 매니저 등록
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.ObjectPoolingManager = this;
        }
    }

    // Runner.Spawn이 호출될 때 실행되는 함수
    public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
    {
        NetworkObject networkObject = null;
        NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out var prefab);
        prefabsThatHadBeenInstantiated.TryGetValue(prefab, out var networkObjects);
        bool foundMatch = false;

        // 비활성화된 오브젝트가 있는지 검색
        if (networkObjects?.Count > 0)
        {
            foreach (var item in networkObjects)
            {
                if (item != null && item.gameObject.activeSelf == false)
                {
                    // 재사용 가능한 오브젝트 발견
                    networkObject = item;
                    foundMatch = true;
                    break;
                }
            }
        }

        // 재사용 가능한 오브젝트가 없거나 새로운 프리팹인 경우 새로 생성
        if (foundMatch == false)
        {
            networkObject = CreateObjectInstance(prefab);
        }

        return networkObject;
    }

    // 새로운 네트워크 오브젝트 생성
    private NetworkObject CreateObjectInstance(NetworkObject prefab)
    {
        var obj = Instantiate(prefab);

        // 딕셔너리에 추가
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

    // Runner.Despawn이 호출될 때 실행되는 함수
    public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
    {
        instance.gameObject.SetActive(false);
    }

    // 딕셔너리에서 네트워크 오브젝트 제거
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