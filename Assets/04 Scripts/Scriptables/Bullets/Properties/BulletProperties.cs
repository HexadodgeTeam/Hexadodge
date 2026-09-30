using UnityEngine;
using Curve = UnityEngine.ParticleSystem.MinMaxCurve;

[CreateAssetMenu(fileName = "BulletProperties", menuName = "Scriptable Objects/Bullets/Properties")]
public class BulletProperties : ScriptableObject
{
    public enum ImpactType
    {
        None,
        Damage,
        Heal
    }

    [Tooltip("None : le projectile n'a pas de collision.\n" +
             "Damage : inflige des dégâts au contact.\n" +
             "Heal : soigne au contact.")]
    [EnumButtons] public ImpactType impactType = ImpactType.Damage;
    
    [Tooltip("La puissance de l'impact au contact (dégâts, soin, etc. selon le type d'impact).\n" +
             "Quel que soit le type d'impact, doit être positif pour fonctionner correctement.")]
    [SerializeField] private Curve impactPower = 1;
    public float GetImpactPower(float time, float random) => impactPower.Evaluate(time, random);
    
    [Tooltip("Durée de vie du bullet. Il est toujours détruit après ce laps de temps.")]
    [SerializeField] private Curve lifetime = 5f;
    public float GetLifetime(float random) => impactPower.Evaluate(1f, random);
}
