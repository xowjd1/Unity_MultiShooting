using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private LoadingCavnasController loadingCanvasControllerPrefab;
    [SerializeField] private LobbyPanelBase[] lobbyPanels;

    private void Start()
    {
        foreach (var lobby in lobbyPanels)
        {
            lobby.InitPanel(this);
        }

        Instantiate(loadingCanvasControllerPrefab);
    }

    public void ShowPanel(LobbyPanelBase.LobbyPanelType type)
    {
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