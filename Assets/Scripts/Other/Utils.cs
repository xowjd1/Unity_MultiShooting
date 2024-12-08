using System.Collections;
using Fusion;
using UnityEngine;

public static class Utils 
{
    public static bool IsLocalPlayer(NetworkObject networkObj)
    {
        return networkObj.IsValid == networkObj.HasInputAuthority;
    }
    
    public static IEnumerator PlayAnimAndSetStateWhenFinished(GameObject parent, Animator animator, string clipName,
        bool activeStateAtTheEnd = true)
    {
        animator.Play(clipName);
        var animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(animationLength);
        parent.SetActive(activeStateAtTheEnd);
    }
}
