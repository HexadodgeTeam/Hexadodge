using UnityEngine;

[CreateAssetMenu(fileName = "PSE_FillRange", menuName = "Scriptable Objects/Skills/Passive Effects/FillRange")]
public class PSEFillRange : PassiveSkillEffect
{
    [Tooltip("Multiplicateur du rayon de peinture du joueur.")]
    [SerializeField] private float fillRangeMultiplier = 1f;
    
    public override void OnLevelReached()
    {
        SkillManager.Instance.MultiplyFillRange(fillRangeMultiplier);
    }

    public override void OnAttack()
    {
        SkillManager.Instance.MultiplyFillRange(1f / fillRangeMultiplier);
    }
}