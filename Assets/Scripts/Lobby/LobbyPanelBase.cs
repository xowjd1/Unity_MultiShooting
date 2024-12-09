using UnityEngine;

public class LobbyPanelBase : MonoBehaviour
{
    // 패널 기본 속성
    [field: SerializeField, Header("LobbyPanelBase Vars")]
    public LobbyPanelType PanelType { get; private set; }   // 패널 타입
    [SerializeField] private Animator panelAnimator;         // 패널 애니메이터

    protected LobbyUIManager lobbyUIManager;                 // 로비 UI 매니저 참조

    // 로비 패널 타입 열거형
    public enum LobbyPanelType
    {
        None,
        CreateNicknamePanel,    // 닉네임 생성 패널
        MiddleSectionPanel      // 메인 로비 패널
    }

    // 패널 초기화
    public virtual void InitPanel(LobbyUIManager uiManager)
    {
        lobbyUIManager = uiManager;
    }

    // 패널 표시
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
        const string POP_IN_CLIP_NAME = "In";
        CallAnimationCoroutine(POP_IN_CLIP_NAME, true);
    }

    // 패널 닫기
    protected void ClosePanel()
    {
        const string POP_OUT_CLIP_NAME = "Out";
        CallAnimationCoroutine(POP_OUT_CLIP_NAME, false);
    }

    // 애니메이션 실행
    private void CallAnimationCoroutine(string clipName, bool state)
    {
        StartCoroutine(Utils.PlayAnimAndSetStateWhenFinished(gameObject, panelAnimator, clipName, state));
    }
}