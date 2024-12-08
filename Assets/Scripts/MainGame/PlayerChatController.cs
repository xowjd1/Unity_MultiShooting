using Fusion;
using TMPro;
using UnityEngine;

public class PlayerChatController : NetworkBehaviour
{
    [Networked] public bool IsTyping { get; private set; }
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Animator bubbleAnimator;
    [SerializeField] private TextMeshProUGUI bubbleText;

    public override void Spawned()
    {
        var isLocalPlayer = Object.InputAuthority == Runner.LocalPlayer;
        gameObject.SetActive(isLocalPlayer);

        if (isLocalPlayer)
        {
            inputField.onSelect.AddListener(arg0 => Rpc_UpdateServerTypingStatus(true));
            inputField.onDeselect.AddListener(arg0 => Rpc_UpdateServerTypingStatus(false));
            inputField.onSubmit.AddListener(OnInputFieldSubmit);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_UpdateServerTypingStatus(bool isTyping)
    {
        Debug.Log(isTyping);
        IsTyping = isTyping;
    }

    private void OnInputFieldSubmit(string arg0)
    {
        if (!string.IsNullOrEmpty(arg0))
        {
            RpcSetBubbleSpeech(arg0);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RpcSetBubbleSpeech(NetworkString<_64> txt)
    {
        bubbleText.text = txt.Value;

        const string TRIGGER = "Open";
        bubbleAnimator.SetTrigger(TRIGGER);
    }
}























