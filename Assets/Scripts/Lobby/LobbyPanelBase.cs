using UnityEngine;

public class LobbyPanelBase : MonoBehaviour
{
    // �г� �⺻ �Ӽ�
    [field: SerializeField, Header("LobbyPanelBase Vars")]
    public LobbyPanelType PanelType { get; private set; }   // �г� Ÿ��
    [SerializeField] private Animator panelAnimator;         // �г� �ִϸ�����

    protected LobbyUIManager lobbyUIManager;                 // �κ� UI �Ŵ��� ����

    // �κ� �г� Ÿ�� ������
    public enum LobbyPanelType
    {
        None,
        CreateNicknamePanel,    // �г��� ���� �г�
        MiddleSectionPanel      // ���� �κ� �г�
    }

    // �г� �ʱ�ȭ
    public virtual void InitPanel(LobbyUIManager uiManager)
    {
        lobbyUIManager = uiManager;
    }

    // �г� ǥ��
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
        const string POP_IN_CLIP_NAME = "In";
        CallAnimationCoroutine(POP_IN_CLIP_NAME, true);
    }

    // �г� �ݱ�
    protected void ClosePanel()
    {
        const string POP_OUT_CLIP_NAME = "Out";
        CallAnimationCoroutine(POP_OUT_CLIP_NAME, false);
    }

    // �ִϸ��̼� ����
    private void CallAnimationCoroutine(string clipName, bool state)
    {
        StartCoroutine(Utils.PlayAnimAndSetStateWhenFinished(gameObject, panelAnimator, clipName, state));
    }
}