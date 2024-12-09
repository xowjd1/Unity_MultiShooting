using System.Collections;
using Fusion;
using UnityEngine;

public static class Utils
{
    // 로컬 플레이어인지 확인하는 유틸리티 메서드
    public static bool IsLocalPlayer(NetworkObject networkObj)
    {
        return networkObj.IsValid == networkObj.HasInputAuthority;
    }

    // 애니메이션 재생 후 오브젝트 상태 변경을 처리하는 코루틴
    public static IEnumerator PlayAnimAndSetStateWhenFinished(
        GameObject parent,          // 부모 게임오브젝트
        Animator animator,          // 애니메이터 컴포넌트
        string clipName,           // 재생할 애니메이션 클립 이름
        bool activeStateAtTheEnd = true)  // 애니메이션 종료 후 활성화 상태
    {
        // 애니메이션 재생
        animator.Play(clipName);

        // 현재 애니메이션의 길이만큼 대기
        var animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(animationLength);

        // 애니메이션 종료 후 오브젝트 상태 설정
        parent.SetActive(activeStateAtTheEnd);
    }
}