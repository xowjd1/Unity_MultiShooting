using System;
using Fusion;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // ���� ���� �̺�Ʈ
    public event Action OnGameIsOver;

    // ��ġ ���� ���¸� �����ϴ� ���� ����
    public static bool MatchIsOver { get; private set; }

    // ī�޶� ��� ���� (Inspector���� �Ҵ�)
    [field: SerializeField] public Collider2D CameraBounds { get; private set; }

    // �ʿ��� ������Ʈ ����
    [SerializeField] private Camera cam;                           // ���� ī�޶�
    [SerializeField] private TextMeshProUGUI timerText;           // Ÿ�̸� UI �ؽ�Ʈ
    [SerializeField] private float matchTimerAmount = 60;         // ��ġ �ð�(��)

    // ��Ʈ��ũ ����ȭ�Ǵ� ��ġ Ÿ�̸�
    [Networked] private TickTimer matchTimer { get; set; }

    // ������Ʈ �ʱ�ȭ
    private void Awake()
    {
        // �۷ι� �Ŵ����� �� GameManager ���
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.GameManager = this;
        }
    }

    // ��Ʈ��ũ ������Ʈ ���� �� ȣ��
    public override void Spawned()
    {
        // ��ġ ���� �ʱ�ȭ
        MatchIsOver = false;

        // ī�޶� ��Ȱ��ȭ
        cam.gameObject.SetActive(false);

        // ��ġ Ÿ�̸� ����
        matchTimer = TickTimer.CreateFromSeconds(Runner, matchTimerAmount);
    }

    // ��Ʈ��ũ ����ȭ�� ���� ������Ʈ
    public override void FixedUpdateNetwork()
    {
        // Ÿ�̸Ӱ� ���� ���̰� ���� �ð��� �ִ� ���
        if (matchTimer.Expired(Runner) == false && matchTimer.RemainingTime(Runner).HasValue)
        {
            // ���� �ð��� ��:�� �������� ��ȯ
            var timeSpan = TimeSpan.FromSeconds(matchTimer.RemainingTime(Runner).Value);
            var outPut = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            // UI ������Ʈ
            timerText.text = outPut;
        }
        // Ÿ�̸Ӱ� ����� ���
        else if (matchTimer.Expired(Runner))
        {
            // ��ġ ���� ó��
            MatchIsOver = true;
            matchTimer = TickTimer.None;
            // ���� ���� �̺�Ʈ �߻�
            OnGameIsOver?.Invoke();
            Debug.Log("Match timer had ended");
        }
    }
}