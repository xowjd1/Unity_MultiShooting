using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    // 필요한 프리팹과 패널 참조
    [SerializeField] private LoadingCavnasController loadingCanvasControllerPrefab;  // 로딩 캔버스 프리팹
    [SerializeField] private LobbyPanelBase[] lobbyPanels;  // 로비 패널 배열

    // 초기화
    private void Start()
    {
        // 모든 로비 패널 초기화
        foreach (var lobby in lobbyPanels)
        {
            lobby.InitPanel(this);
        }

        // 로딩 캔버스 생성
        Instantiate(loadingCanvasControllerPrefab);
    }

    // 특정 타입의 패널 표시
    public void ShowPanel(LobbyPanelBase.LobbyPanelType type)
    {
        // 요청된 타입의 패널 찾아서 표시
        foreach (var lobby in lobbyPanels)
        {
            if (lobby.PanelType == type)
            {
                lobby.ShowPanel();
                break;
            }
        }
    }
}