using UnityEngine;

public abstract class PassiveSkillEffect : ScriptableObject
{
    public abstract void OnLevelReached();

    public abstract void OnAttack();
}