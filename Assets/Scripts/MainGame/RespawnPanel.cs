using Fusion;
using TMPro;
using UnityEngine;

public class RespawnPanel : SimulationBehaviour
{
    // UI ���� ������Ʈ
    [SerializeField] private PlayerController playerController;     // �÷��̾� ��Ʈ�ѷ� ����
    [SerializeField] private TextMeshProUGUI respawnAmountText;    // ������ �ð� ǥ�� �ؽ�Ʈ
    [SerializeField] private GameObject childObj;                   // ������ �г� ������Ʈ

    // ��Ʈ��ũ ������Ʈ
    public override void FixedUpdateNetwork()
    {
        // ���� �÷��̾��� ��츸 ����
        if (Utils.IsLocalPlayer(Object))
        {
            // ������ Ÿ�̸� ���� ������ üũ
            var timerIsRunning = playerController.RespawnTimer.IsRunning;
            childObj.SetActive(timerIsRunning);

            // Ÿ�̸Ӱ� ���� ���̰� ���� �ð��� �ִ� ���
            if (timerIsRunning && playerController.RespawnTimer.RemainingTime(Runner).HasValue)
            {
                // ���� �ð��� ������ �ݿø��Ͽ� ǥ��
                var time = playerController.RespawnTimer.RemainingTime(Runner).Value;
                var roundInt = Mathf.RoundToInt(time);
                respawnAmountText.text = roundInt.ToString();
            }
        }
    }
}