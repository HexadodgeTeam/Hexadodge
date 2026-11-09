using UnityEngine;
[CreateAssetMenu(fileName = "S_Skill", menuName = "Scriptable Objects/Skills/Skill")]
public class SkillData : ScriptableObject
{
  public string Name;
  public string Description;
  public Sprite Icon;
  public string lore;
  public int ID;
  
  [Tooltip("Palier de la compétence (commence à 1 pour le premier palier).\n" +
           "Si 0, est actif en continu.")]
  public int level;

  [Tooltip("Effets actifs de la compétence (se déclenchent à l'attaque).")]
  public ActiveSkillEffect[] activeEffects;
    
  [Tooltip("Effets passifs de la compétence (se déclenchent quand le palier est débloqué).")]
  public PassiveSkillEffect[] passiveEffects;

  public void OnLevelReached(int levelFrom0)
  {
    if (levelFrom0 + 1 == level)
    {
      foreach (PassiveSkillEffect passive in passiveEffects)
      {
        passive.OnLevelReached();
      }
    }
  }

  public void OnAttack(int levelFrom0)
  {
    if (levelFrom0 + 1 >= level)
    {
      foreach (ActiveSkillEffect active in activeEffects)
      {
        active.OnAttack();
      }

      foreach (PassiveSkillEffect passive in passiveEffects)
      {
        passive.OnAttack();
      }
    }
  }
}
