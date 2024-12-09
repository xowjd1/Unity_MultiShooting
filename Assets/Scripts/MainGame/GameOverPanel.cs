using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    // UI 컴포넌트 참조
    [SerializeField] private Button returnToLobbyBtn;    // 로비 복귀 버튼
    [SerializeField] private GameObject childObj;        // 게임오버 패널 오브젝트

    // 초기화    
    private void Start()
    {
        // 게임 종료 이벤트 리스너 등록
        GlobalManagers.Instance.GameManager.OnGameIsOver += OnMatchIsOver;

        // 로비 복귀 버튼 클릭 이벤트 설정
        returnToLobbyBtn.onClick.AddListener(() =>
            GlobalManagers.Instance.NetworkRunnerController.ShutDownRunner());
    }

    // 게임 종료 시 호출
    private void OnMatchIsOver()
    {
        childObj.SetActive(true);  // 게임오버 패널 표시
    }

    // 컴포넌트 제거 시 이벤트 리스너 해제
    private void OnDestroy()
    {
        GlobalManagers.Instance.GameManager.OnGameIsOver -= OnMatchIsOver;
    }
}