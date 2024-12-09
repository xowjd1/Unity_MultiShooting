using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiddleSectionPanel : LobbyPanelBase
{
    // UI ������Ʈ ����
    [Header("MiddleSectionPanel Vars")]
    [SerializeField] private Button joinRandomRoomBtn;          // ���� �� ���� ��ư
    [SerializeField] private Button joinRoomByArgBtn;           // �� �̸����� ���� ��ư
    [SerializeField] private Button createRoomBtn;              // �� ���� ��ư
    [SerializeField] private TMP_InputField joinRoomByArgInputField;   // ������ �� �̸� �Է�
    [SerializeField] private TMP_InputField createRoomInputField;      // ������ �� �̸� �Է�

    private NetworkRunnerController networkRunnerController;

    // �г� �ʱ�ȭ
    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);

        // ��Ʈ��ũ ���� ��Ʈ�ѷ� ����
        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;

        // ��ư �̺�Ʈ ������ ���
        joinRandomRoomBtn.onClick.AddListener(JoinRandomRoom);
        joinRoomByArgBtn.onClick.AddListener(() =>
            CreateRoom(GameMode.Client, joinRoomByArgInputField.text));
        createRoomBtn.onClick.AddListener(() =>
            CreateRoom(GameMode.Host, createRoomInputField.text));
    }

    // �� ����/����
    private void CreateRoom(GameMode mode, string field)
    {
        // �� �̸��� 2���� �̻��� ��쿡�� ó��
        if (field.Length >= 2)
        {
            Debug.Log($"------------{mode}------------");
            networkRunnerController.StartGame(mode, field);
        }
    }

    // ���� �� ����
    private void JoinRandomRoom()
    {
        Debug.Log($"------------JoinRandomRoom!------------");
        networkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
    }
}