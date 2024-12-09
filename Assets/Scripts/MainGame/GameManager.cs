using System;
using Fusion;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // 게임 종료 이벤트
    public event Action OnGameIsOver;

    // 매치 종료 상태를 저장하는 정적 변수
    public static bool MatchIsOver { get; private set; }

    // 카메라 경계 설정 (Inspector에서 할당)
    [field: SerializeField] public Collider2D CameraBounds { get; private set; }

    // 필요한 컴포넌트 참조
    [SerializeField] private Camera cam;                           // 메인 카메라
    [SerializeField] private TextMeshProUGUI timerText;           // 타이머 UI 텍스트
    [SerializeField] private float matchTimerAmount = 60;         // 매치 시간(초)

    // 네트워크 동기화되는 매치 타이머
    [Networked] private TickTimer matchTimer { get; set; }

    // 컴포넌트 초기화
    private void Awake()
    {
        // 글로벌 매니저에 이 GameManager 등록
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.GameManager = this;
        }
    }

    // 네트워크 오브젝트 생성 시 호출
    public override void Spawned()
    {
        // 매치 상태 초기화
        MatchIsOver = false;

        // 카메라 비활성화
        cam.gameObject.SetActive(false);

        // 매치 타이머 설정
        matchTimer = TickTimer.CreateFromSeconds(Runner, matchTimerAmount);
    }

    // 네트워크 동기화된 물리 업데이트
    public override void FixedUpdateNetwork()
    {
        // 타이머가 실행 중이고 남은 시간이 있는 경우
        if (matchTimer.Expired(Runner) == false && matchTimer.RemainingTime(Runner).HasValue)
        {
            // 남은 시간을 분:초 형식으로 변환
            var timeSpan = TimeSpan.FromSeconds(matchTimer.RemainingTime(Runner).Value);
            var outPut = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            // UI 업데이트
            timerText.text = outPut;
        }
        // 타이머가 종료된 경우
        else if (matchTimer.Expired(Runner))
        {
            // 매치 종료 처리
            MatchIsOver = true;
            matchTimer = TickTimer.None;
            // 게임 종료 이벤트 발생
            OnGameIsOver?.Invoke();
            Debug.Log("Match timer had ended");
        }
    }
}