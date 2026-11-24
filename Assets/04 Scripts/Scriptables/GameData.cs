using System;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "Game", menuName = "Scriptable Objects/Game")]
public class GameData : ScriptableObject
{
    public FightParameters fight;
    
    [Tooltip("Tous les skills du jeu. Un skill doit être enregistré ici pour fonctionner correctement.")]
    public SkillData[] gameSkills;

    [Tooltip("Ces skills sont donnés au début d'une nouvelle sauvegarde.")]
    public SkillData[] startSkills;

    public SkillData GetSkillByID(int id)
    {
        foreach (SkillData s in gameSkills)
        {
            if (s.ID == id)
            {
                return s;
            }
        }

        return null;
    }
}

[Serializable]
public class FightParameters
{
    public int[] attackPowers;
    [Min(0f)] public float attackSlowdownTimeScale = 0.1f;
    public AnimationCurve attackSlowdownIn = new(new Keyframe(0, 0), new Keyframe(1, 1));
    public float attackSlowdownInDuration = 0.3f;
    public AnimationCurve attackSlowdownOut = new(new Keyframe(0, 0), new Keyframe(1, 1));
    public float attackSlowdownOutDuration = 0.2f;
}