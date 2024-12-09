using UnityEngine;
using UnityEngine.UI;

public class LoadingCavnasController : MonoBehaviour
{
    // UI ������Ʈ
    [SerializeField] private Animator animator;     // �ε� �ִϸ��̼�
    [SerializeField] private Button cancelBtn;      // ��� ��ư

    private NetworkRunnerController networkRunnerController;

    // �ʱ�ȭ
    private void Start()
    {
        // ��Ʈ��ũ ���� ��Ʈ�ѷ� ���� �� �̺�Ʈ ������ ���
        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;
        networkRunnerController.OnStartedRunnerConnection += OnStartedRunnerConnection;
        networkRunnerController.OnPlayerJoinedSuccessfully += OnPlayerJoinedSuccessfully;

        // ��� ��ư �̺�Ʈ ����
        cancelBtn.onClick.AddListener(networkRunnerController.ShutDownRunner);
        this.gameObject.SetActive(false);
    }

    // ���� ���� ���� �� ȣ��
    private void OnStartedRunnerConnection()
    {
        this.gameObject.SetActive(true);
        const string CLIP_NAME = "In";
        // ���̵� �� �ִϸ��̼� ���
        StartCoroutine(Utils.PlayAnimAndSetStateWhenFinished(gameObject, animator, CLIP_NAME));
    }

    // �÷��̾� ���� ���� �� ȣ��
    private void OnPlayerJoinedSuccessfully()
    {
        const string CLIP_NAME = "Out";
        // ���̵� �ƿ� �ִϸ��̼� ��� �� ��Ȱ��ȭ
        StartCoroutine(Utils.PlayAnimAndSetStateWhenFinished(gameObject, animator, CLIP_NAME, false));
    }

    // ������Ʈ ���� �� �̺�Ʈ ������ ����
    private void OnDestroy()
    {
        networkRunnerController.OnStartedRunnerConnection -= OnStartedRunnerConnection;
        networkRunnerController.OnPlayerJoinedSuccessfully -= OnPlayerJoinedSuccessfully;
    }
}