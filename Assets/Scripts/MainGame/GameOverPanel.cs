using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    // UI ������Ʈ ����
    [SerializeField] private Button returnToLobbyBtn;    // �κ� ���� ��ư
    [SerializeField] private GameObject childObj;        // ���ӿ��� �г� ������Ʈ

    // �ʱ�ȭ    
    private void Start()
    {
        // ���� ���� �̺�Ʈ ������ ���
        GlobalManagers.Instance.GameManager.OnGameIsOver += OnMatchIsOver;

        // �κ� ���� ��ư Ŭ�� �̺�Ʈ ����
        returnToLobbyBtn.onClick.AddListener(() =>
            GlobalManagers.Instance.NetworkRunnerController.ShutDownRunner());
    }

    // ���� ���� �� ȣ��
    private void OnMatchIsOver()
    {
        childObj.SetActive(true);  // ���ӿ��� �г� ǥ��
    }

    // ������Ʈ ���� �� �̺�Ʈ ������ ����
    private void OnDestroy()
    {
        GlobalManagers.Instance.GameManager.OnGameIsOver -= OnMatchIsOver;
    }
}