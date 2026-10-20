using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player/Data")]
public class PlayerData : ScriptableObject
{
    public PlayerMovementData movementData;
    public float fillRange = 2.1f;
    public PlayerHealth.Data healthData;
}