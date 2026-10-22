using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Game", menuName = "Scriptable Objects/Game")]
public class GameData : ScriptableObject
{
    public GameParameters fight;
}

[Serializable]
public class GameParameters
{
    public int[] attackPowers;
    [Min(0f)] public float attackSlowdownTimeScale = 0.1f;
    public AnimationCurve attackSlowdownIn = new(new Keyframe(0, 0), new Keyframe(1, 1));
    public float attackSlowdownInDuration = 0.3f;
    public AnimationCurve attackSlowdownOut = new(new Keyframe(0, 0), new Keyframe(1, 1));
    public float attackSlowdownOutDuration = 0.2f;
}