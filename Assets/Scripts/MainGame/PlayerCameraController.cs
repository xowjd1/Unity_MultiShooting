using Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner2D;

    private void Start()
    {
        // 게임 매니저에서 설정된 카메라 경계 값을 가져와 적용
        cinemachineConfiner2D.m_BoundingShape2D = GlobalManagers.Instance.GameManager.CameraBounds;
    }

    // 외부에서 호출하여 카메라 흔들림 효과 생성
    public void ShakeCamera(Vector3 shakeAmount)
    {
        impulseSource.GenerateImpulse(shakeAmount);
    }
}