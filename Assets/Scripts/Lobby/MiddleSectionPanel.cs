using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiddleSectionPanel : LobbyPanelBase
{
    // UI 컴포넌트 참조
    [Header("MiddleSectionPanel Vars")]
    [SerializeField] private Button joinRandomRoomBtn;          // 랜덤 방 참가 버튼
    [SerializeField] private Button joinRoomByArgBtn;           // 방 이름으로 참가 버튼
    [SerializeField] private Button createRoomBtn;              // 방 생성 버튼
    [SerializeField] private TMP_InputField joinRoomByArgInputField;   // 참가할 방 이름 입력
    [SerializeField] private TMP_InputField createRoomInputField;      // 생성할 방 이름 입력

    private NetworkRunnerController networkRunnerController;

    // 패널 초기화
    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);

        // 네트워크 러너 컨트롤러 참조
        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;

        // 버튼 이벤트 리스너 등록
        joinRandomRoomBtn.onClick.AddListener(JoinRandomRoom);
        joinRoomByArgBtn.onClick.AddListener(() =>
            CreateRoom(GameMode.Client, joinRoomByArgInputField.text));
        createRoomBtn.onClick.AddListener(() =>
            CreateRoom(GameMode.Host, createRoomInputField.text));
    }

    // 방 생성/참가
    private void CreateRoom(GameMode mode, string field)
    {
        // 방 이름이 2글자 이상인 경우에만 처리
        if (field.Length >= 2)
        {
            Debug.Log($"------------{mode}------------");
            networkRunnerController.StartGame(mode, field);
        }
    }

    // 랜덤 방 참가
    private void JoinRandomRoom()
    {
        Debug.Log($"------------JoinRandomRoom!------------");
        networkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
    }
}