using UnityEngine;

[CreateAssetMenu(fileName = "ASE_ClearBullets", menuName = "Scriptable Objects/Skills/Active Effects/Clear Bullets")]
public class ASEClearBullets : ActiveSkillEffect
{
    public override void OnAttack()
    {
        SkillManager.Instance.ClearBullets();
    }
}