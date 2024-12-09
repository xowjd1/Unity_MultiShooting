using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    // �ʿ��� �����հ� �г� ����
    [SerializeField] private LoadingCavnasController loadingCanvasControllerPrefab;  // �ε� ĵ���� ������
    [SerializeField] private LobbyPanelBase[] lobbyPanels;  // �κ� �г� �迭

    // �ʱ�ȭ
    private void Start()
    {
        // ��� �κ� �г� �ʱ�ȭ
        foreach (var lobby in lobbyPanels)
        {
            lobby.InitPanel(this);
        }

        // �ε� ĵ���� ����
        Instantiate(loadingCanvasControllerPrefab);
    }

    // Ư�� Ÿ���� �г� ǥ��
    public void ShowPanel(LobbyPanelBase.LobbyPanelType type)
    {
        // ��û�� Ÿ���� �г� ã�Ƽ� ǥ��
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