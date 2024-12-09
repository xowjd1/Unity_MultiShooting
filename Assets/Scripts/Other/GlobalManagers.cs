using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GlobalManagers Instance { get; private set; }

    // 매니저 오브젝트
    [SerializeField] private GameObject parentObj;  // 매니저들의 부모 오브젝트

    // 다른 매니저 컴포넌트 참조
    [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }  // 네트워크 러너 컨트롤러
    public PlayerSpawnerController PlayerSpawnerController { get; set; }   // 플레이어 스폰 컨트롤러
    public ObjectPoolingManager ObjectPoolingManager { get; set; }         // 오브젝트 풀링 매니저 
    public GameManager GameManager { get; set; }                          // 게임 매니저

    // 초기화
    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 이미 인스턴스가 존재하면 현재 오브젝트 제거
            Destroy(parentObj);
        }
    }
}