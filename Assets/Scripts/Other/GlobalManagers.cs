using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    public static GlobalManagers Instance { get; private set; }

    [SerializeField] private GameObject parentObj;
    [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }
    public PlayerSpawnerController PlayerSpawnerController { get; set; }
    public ObjectPoolingManager ObjectPoolingManager { get; set; }
    public GameManager GameManager { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(parentObj);
        }
    }
}