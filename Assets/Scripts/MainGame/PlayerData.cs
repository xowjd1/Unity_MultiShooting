using Fusion;
using UnityEngine;

public struct PlayerData : INetworkInput
{
    public float HorizontalInput;
    public Quaternion GunPivotRotation;
    public NetworkButtons NetworkButtons;
}
