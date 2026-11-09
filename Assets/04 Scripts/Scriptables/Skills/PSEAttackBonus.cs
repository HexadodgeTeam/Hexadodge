using UnityEngine;

[CreateAssetMenu(fileName = "PSE_AttackBonus", menuName = "Scriptable Objects/Skills/Passive Effects/Attack Bonus")]
public class PSEAttackBonus : PassiveSkillEffect
{
    [Tooltip("Bonus d'attaque du joueur. Addition.")]
    [SerializeField] private int attackBonus = 1;
    
    public override void OnLevelReached()
    {
        SkillManager.Instance.AddAttackBonus(attackBonus);
    }

    public override void OnAttack()
    {
        SkillManager.Instance.AddAttackBonus(-attackBonus);
    }
}