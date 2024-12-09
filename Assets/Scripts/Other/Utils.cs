using System.Collections;
using Fusion;
using UnityEngine;

public static class Utils
{
    // ���� �÷��̾����� Ȯ���ϴ� ��ƿ��Ƽ �޼���
    public static bool IsLocalPlayer(NetworkObject networkObj)
    {
        return networkObj.IsValid == networkObj.HasInputAuthority;
    }

    // �ִϸ��̼� ��� �� ������Ʈ ���� ������ ó���ϴ� �ڷ�ƾ
    public static IEnumerator PlayAnimAndSetStateWhenFinished(
        GameObject parent,          // �θ� ���ӿ�����Ʈ
        Animator animator,          // �ִϸ����� ������Ʈ
        string clipName,           // ����� �ִϸ��̼� Ŭ�� �̸�
        bool activeStateAtTheEnd = true)  // �ִϸ��̼� ���� �� Ȱ��ȭ ����
    {
        // �ִϸ��̼� ���
        animator.Play(clipName);

        // ���� �ִϸ��̼��� ���̸�ŭ ���
        var animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(animationLength);

        // �ִϸ��̼� ���� �� ������Ʈ ���� ����
        parent.SetActive(activeStateAtTheEnd);
    }
}