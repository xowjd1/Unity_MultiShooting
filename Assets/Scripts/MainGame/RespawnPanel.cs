using Fusion;
using TMPro;
using UnityEngine;

public class RespawnPanel : SimulationBehaviour
{
    // UI 관련 컴포넌트
    [SerializeField] private PlayerController playerController;     // 플레이어 컨트롤러 참조
    [SerializeField] private TextMeshProUGUI respawnAmountText;    // 리스폰 시간 표시 텍스트
    [SerializeField] private GameObject childObj;                   // 리스폰 패널 오브젝트

    // 네트워크 업데이트
    public override void FixedUpdateNetwork()
    {
        // 로컬 플레이어인 경우만 실행
        if (Utils.IsLocalPlayer(Object))
        {
            // 리스폰 타이머 실행 중인지 체크
            var timerIsRunning = playerController.RespawnTimer.IsRunning;
            childObj.SetActive(timerIsRunning);

            // 타이머가 실행 중이고 남은 시간이 있는 경우
            if (timerIsRunning && playerController.RespawnTimer.RemainingTime(Runner).HasValue)
            {
                // 남은 시간을 정수로 반올림하여 표시
                var time = playerController.RespawnTimer.RemainingTime(Runner).Value;
                var roundInt = Mathf.RoundToInt(time);
                respawnAmountText.text = roundInt.ToString();
            }
        }
    }
}