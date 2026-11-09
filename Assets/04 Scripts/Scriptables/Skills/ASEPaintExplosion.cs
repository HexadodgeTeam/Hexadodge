using UnityEngine;

[CreateAssetMenu(fileName = "ASE_PaintExplosion", menuName = "Scriptable Objects/Skills/Active Effects/Paint Explosion")]
public class ASEPaintExplosion : ActiveSkillEffect
{
    [SerializeField] private float radius;
    
    public override void OnAttack()
    {
        SkillManager.Instance.PaintExplosion(radius);
    }
}