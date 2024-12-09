using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GlobalManagers Instance { get; private set; }

    // �Ŵ��� ������Ʈ
    [SerializeField] private GameObject parentObj;  // �Ŵ������� �θ� ������Ʈ

    // �ٸ� �Ŵ��� ������Ʈ ����
    [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }  // ��Ʈ��ũ ���� ��Ʈ�ѷ�
    public PlayerSpawnerController PlayerSpawnerController { get; set; }   // �÷��̾� ���� ��Ʈ�ѷ�
    public ObjectPoolingManager ObjectPoolingManager { get; set; }         // ������Ʈ Ǯ�� �Ŵ��� 
    public GameManager GameManager { get; set; }                          // ���� �Ŵ���

    // �ʱ�ȭ
    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // �̹� �ν��Ͻ��� �����ϸ� ���� ������Ʈ ����
            Destroy(parentObj);
        }
    }
}