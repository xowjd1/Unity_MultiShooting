using Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner2D;

    private void Start()
    {
        cinemachineConfiner2D.m_BoundingShape2D = GlobalManagers.Instance.GameManager.CameraBounds;
    }

    public void ShakeCamera(Vector3 shakeAmount)
    {
        impulseSource.GenerateImpulse(shakeAmount);
    }
}
