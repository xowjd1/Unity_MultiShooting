using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNickNamePanel : LobbyPanelBase
{
    // UI ������Ʈ ����
    [Header("CreateNickNamePanel Vars")]
    [SerializeField] private TMP_InputField inputField;      // �г��� �Է� �ʵ�
    [SerializeField] private Button createNicknameBtn;       // �г��� ���� ��ư

    // �г��� �ּ� ���� ���
    private const int MAX_CHAR_FOR_NICKNAME = 2;

    // �г� �ʱ�ȭ
    public override void InitPanel(LobbyUIManager lobbyUIManager)
    {
        base.InitPanel(lobbyUIManager);

        // ��ư �ʱ� ���� ���� �� �̺�Ʈ ������ ���
        createNicknameBtn.interactable = false;
        createNicknameBtn.onClick.AddListener(OnClickCreateNickname);
        inputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    // �Է°� ���� �� ȣ��
    private void OnInputValueChanged(string arg0)
    {
        // �Էµ� �ؽ�Ʈ ���̿� ���� ��ư Ȱ��ȭ/��Ȱ��ȭ
        createNicknameBtn.interactable = arg0.Length >= MAX_CHAR_FOR_NICKNAME;
    }

    // �г��� ���� ��ư Ŭ�� �� ȣ��
    private void OnClickCreateNickname()
    {
        var nickName = inputField.text;
        if (nickName.Length >= MAX_CHAR_FOR_NICKNAME)
        {
            // �г��� ���� �� �г� ��ȯ
            GlobalManagers.Instance.NetworkRunnerController.SetPlayerNickname(nickName);
            base.ClosePanel();
            lobbyUIManager.ShowPanel(LobbyPanelType.MiddleSectionPanel);
        }
    }
}