using Fusion;
using TMPro;
using UnityEngine;

public class PlayerChatController : NetworkBehaviour
{
    // �÷��̾��� ä�� �Է� ���¸� ��Ʈ��ũ�� ����ȭ
    [Networked] public bool IsTyping { get; private set; }

    // UI ������Ʈ ����
    [SerializeField] private TMP_InputField inputField;      // ä�� �Է� �ʵ�
    [SerializeField] private Animator bubbleAnimator;        // ��ǳ�� �ִϸ�����
    [SerializeField] private TextMeshProUGUI bubbleText;     // ��ǳ�� �ؽ�Ʈ

    public override void Spawned()
    {
        // ���� �÷��̾����� Ȯ��
        var isLocalPlayer = Object.InputAuthority == Runner.LocalPlayer;
        gameObject.SetActive(isLocalPlayer);

        // ���� �÷��̾��� ��쿡�� �Է� �̺�Ʈ ������ ����
        if (isLocalPlayer)
        {
            // ä��â ��Ŀ��/���� �� Ÿ���� ���� ������Ʈ
            inputField.onSelect.AddListener(arg0 => Rpc_UpdateServerTypingStatus(true));
            inputField.onDeselect.AddListener(arg0 => Rpc_UpdateServerTypingStatus(false));
            // ä�� �Է� �Ϸ� �� �̺�Ʈ
            inputField.onSubmit.AddListener(OnInputFieldSubmit);
        }
    }

    // ������ Ÿ���� ���� ������Ʈ ��û
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_UpdateServerTypingStatus(bool isTyping)
    {
        Debug.Log(isTyping);
        IsTyping = isTyping;
    }

    // ä�� �Է� �Ϸ� ó��
    private void OnInputFieldSubmit(string arg0)
    {
        if (!string.IsNullOrEmpty(arg0))
        {
            RpcSetBubbleSpeech(arg0);
        }
    }

    // ��� Ŭ���̾�Ʈ�� ��ǳ�� �޽��� ǥ�� ��û
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RpcSetBubbleSpeech(NetworkString<_64> txt)
    {
        bubbleText.text = txt.Value;
        const string TRIGGER = "Open";
        bubbleAnimator.SetTrigger(TRIGGER);
    }
}