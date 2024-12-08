using System;
using Fusion;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public event Action OnGameIsOver;
    public static bool MatchIsOver { get; private set; }
    [field: SerializeField] public Collider2D CameraBounds { get; private set; }
    [SerializeField] private Camera cam;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float matchTimerAmount = 60;
    
    [Networked] private TickTimer matchTimer { get; set; }

    private void Awake()
    {
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.GameManager = this;
        }
    }

    public override void Spawned()
    {
        //Reset this var
        MatchIsOver = false;
        
        cam.gameObject.SetActive(false);
        matchTimer = TickTimer.CreateFromSeconds(Runner, matchTimerAmount);
    }

    public override void FixedUpdateNetwork()
    {
        if (matchTimer.Expired(Runner) == false && matchTimer.RemainingTime(Runner).HasValue)
        {
            var timeSpan = TimeSpan.FromSeconds(matchTimer.RemainingTime(Runner).Value);
            var outPut = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            timerText.text = outPut;
        }
        else if (matchTimer.Expired(Runner))
        {
            MatchIsOver = true;
            matchTimer = TickTimer.None;
            OnGameIsOver?.Invoke();
            Debug.Log("Match timer had ended");
        }
    }
}
 





















