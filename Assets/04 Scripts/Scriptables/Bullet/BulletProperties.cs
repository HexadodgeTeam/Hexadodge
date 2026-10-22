using System;
using UnityEngine;

[Serializable]
public class BulletProperties
{
    public enum ImpactType
    {
        None,
        Damage,
        Heal
    }

    public enum ExplosionDirection
    {
        BulletDirection,
        Player,
        Random
    }

    [Tooltip("Durée de vie du bullet. Il est toujours détruit après ce laps de temps.\n" +
             "Utiliser une curve ici n'a aucun effet.")]
    [SerializeField]
    internal FloatCurve lifetime = 5f;

    [Tooltip("Durée avant que le bullet ne soit tangible.")]
    [SerializeField]
    internal float intangibleStart = 0.1f;

    [Tooltip("Durée d'intangibilité du bullet avant sa disparition.")]
    [SerializeField]
    internal float intangibleEnd = 0.1f;
    
    [Tooltip("None : le projectile n'a pas de collision.\n" +
             "Damage : inflige des dégâts au contact.\n" +
             "Heal : soigne au contact.")]
    [EnumButtons]
    public ImpactType impactType = ImpactType.Damage;

    [Tooltip("Si la bullet se détruit quand elle collisionne avec le joueur.")]
    [SerializeField]
    internal bool destroyOnImpact = true;

    [Tooltip("La puissance de l'impact au contact (dégâts, soin, etc. selon le type d'impact).\n" +
             "Quel que soit le type d'impact, doit être positif pour fonctionner correctement.")]
    [SerializeField]
    internal IntCurve impactPower = 1;

    public Explosion explosion;

    [Serializable]
    public class Explosion
    {
        [Tooltip("Le nombre de bullets libérés à la disparition.")] [SerializeField]
        internal IntCurve bulletCount = 0;

        [Tooltip("Les bullets issus de la disparition du projectile.\n" +
                 "Si plusieurs sont définis, ils seront choisis au hasard.\n" +
                 "Si aucun n'est défini, les projectiles émis seront des clones de ce projectile.")]
        public BulletData[] bullets;

        [Tooltip("L'arc de l'explosion en degré. L'arc est dirigé vers Explosion Direction (voir ci-dessous).")]
        [SerializeField]
        internal FloatCurve arc = 360;

        [Tooltip("Décalage de la rotation de l'explosion.")] [SerializeField]
        internal FloatCurve rotationOffset = 0;

        [Tooltip("La direction de l'explosion, au cas où l'arc est à moins de 360 degrés.\n" +
                 "BulletDirection : la direction qu'avait le bullet avant d'exploser.\n" +
                 "Player : vers le joueur.\n" +
                 "Random : aléatoire.")]
        public ExplosionDirection direction = ExplosionDirection.Player;
    }

    public Collider collider;

    [Serializable]
    public class Collider
    {
        public enum Shape
        {
            Rectangle,
            Circle
        }
        
        [Tooltip("Forme du collider. Rectangle ou cercle.")]
        public Shape shape = Shape.Rectangle;

        [Tooltip("Dimensions du collider si c'est un rectangle.")]
        public Vector2 rectangleSize = new(1, 1);

        [Tooltip("Rayon du collider si c'est un cercle.")]
        public float circleRadius = 0.5f;

        [Tooltip("Si le collider possède des collisions. Si oui, il ne provoquera pas d'impact au contact du joueur.")]
        public bool isWall;
    }

    public BulletPropertiesInstance Instance() => new(this);
}

public class BulletPropertiesInstance
{
    public FloatCurveInstance Lifetime;
    public IntCurveInstance ImpactPower;
    public IntCurveInstance ExplosionBulletCount;
    public FloatCurveInstance ExplosionArc;
    public FloatCurveInstance RotationOffset;

    public BulletPropertiesInstance(BulletProperties data)
    {
        Lifetime = data.lifetime.Instance();
        ImpactPower = data.impactPower.Instance();
        ExplosionBulletCount = data.explosion.bulletCount.Instance();
        ExplosionArc = data.explosion.arc.Instance();
        RotationOffset = data.explosion.rotationOffset.Instance();
    }

    public void ReRoll()
    {
        Lifetime.ReRoll();
        ImpactPower.ReRoll();
        ExplosionBulletCount.ReRoll();
        ExplosionArc.ReRoll();
        RotationOffset.ReRoll();
    }
}