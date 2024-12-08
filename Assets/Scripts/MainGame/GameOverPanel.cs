using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button returnToLobbyBtn;
    [SerializeField] private GameObject childObj;
        
    private void Start()
    {
        GlobalManagers.Instance.GameManager.OnGameIsOver += OnMatchIsOver;
        returnToLobbyBtn.onClick.AddListener(() => GlobalManagers.Instance.NetworkRunnerController.ShutDownRunner());
    }

    private void OnMatchIsOver()
    {
        childObj.SetActive(true);
    }

    private void OnDestroy()
    {
        GlobalManagers.Instance.GameManager.OnGameIsOver -= OnMatchIsOver;
    }
}
