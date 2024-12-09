using Fusion;
using TMPro;
using UnityEngine;

public class PlayerChatController : NetworkBehaviour
{
    // 플레이어의 채팅 입력 상태를 네트워크로 동기화
    [Networked] public bool IsTyping { get; private set; }

    // UI 컴포넌트 참조
    [SerializeField] private TMP_InputField inputField;      // 채팅 입력 필드
    [SerializeField] private Animator bubbleAnimator;        // 말풍선 애니메이터
    [SerializeField] private TextMeshProUGUI bubbleText;     // 말풍선 텍스트

    public override void Spawned()
    {
        // 로컬 플레이어인지 확인
        var isLocalPlayer = Object.InputAuthority == Runner.LocalPlayer;
        gameObject.SetActive(isLocalPlayer);

        // 로컬 플레이어인 경우에만 입력 이벤트 리스너 설정
        if (isLocalPlayer)
        {
            // 채팅창 포커스/해제 시 타이핑 상태 업데이트
            inputField.onSelect.AddListener(arg0 => Rpc_UpdateServerTypingStatus(true));
            inputField.onDeselect.AddListener(arg0 => Rpc_UpdateServerTypingStatus(false));
            // 채팅 입력 완료 시 이벤트
            inputField.onSubmit.AddListener(OnInputFieldSubmit);
        }
    }

    // 서버에 타이핑 상태 업데이트 요청
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_UpdateServerTypingStatus(bool isTyping)
    {
        Debug.Log(isTyping);
        IsTyping = isTyping;
    }

    // 채팅 입력 완료 처리
    private void OnInputFieldSubmit(string arg0)
    {
        if (!string.IsNullOrEmpty(arg0))
        {
            RpcSetBubbleSpeech(arg0);
        }
    }

    // 모든 클라이언트에 말풍선 메시지 표시 요청
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RpcSetBubbleSpeech(NetworkString<_64> txt)
    {
        bubbleText.text = txt.Value;
        const string TRIGGER = "Open";
        bubbleAnimator.SetTrigger(TRIGGER);
    }
}