using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNickNamePanel : LobbyPanelBase
{
    // UI 컴포넌트 참조
    [Header("CreateNickNamePanel Vars")]
    [SerializeField] private TMP_InputField inputField;      // 닉네임 입력 필드
    [SerializeField] private Button createNicknameBtn;       // 닉네임 생성 버튼

    // 닉네임 최소 길이 상수
    private const int MAX_CHAR_FOR_NICKNAME = 2;

    // 패널 초기화
    public override void InitPanel(LobbyUIManager lobbyUIManager)
    {
        base.InitPanel(lobbyUIManager);

        // 버튼 초기 상태 설정 및 이벤트 리스너 등록
        createNicknameBtn.interactable = false;
        createNicknameBtn.onClick.AddListener(OnClickCreateNickname);
        inputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    // 입력값 변경 시 호출
    private void OnInputValueChanged(string arg0)
    {
        // 입력된 텍스트 길이에 따라 버튼 활성화/비활성화
        createNicknameBtn.interactable = arg0.Length >= MAX_CHAR_FOR_NICKNAME;
    }

    // 닉네임 생성 버튼 클릭 시 호출
    private void OnClickCreateNickname()
    {
        var nickName = inputField.text;
        if (nickName.Length >= MAX_CHAR_FOR_NICKNAME)
        {
            // 닉네임 설정 및 패널 전환
            GlobalManagers.Instance.NetworkRunnerController.SetPlayerNickname(nickName);
            base.ClosePanel();
            lobbyUIManager.ShowPanel(LobbyPanelType.MiddleSectionPanel);
        }
    }
}