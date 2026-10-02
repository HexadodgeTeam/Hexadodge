using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/Bullets/Bullet")]
public class BulletData : ScriptableObject
{
    public BulletSpawning spawning;
    public BulletMovement movement;
    public Properties properties;
    
    [Serializable]
    public class Properties
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
    
        [Tooltip("Durée de vie du bullet. Il est toujours détruit après ce laps de temps.")]
        public FloatCurve lifetime = 5f;

        [Tooltip("None : le projectile n'a pas de collision.\n" +
                 "Damage : inflige des dégâts au contact.\n" +
                 "Heal : soigne au contact.")]
        [EnumButtons] public ImpactType impactType = ImpactType.Damage;
    
        [Tooltip("La puissance de l'impact au contact (dégâts, soin, etc. selon le type d'impact).\n" +
                 "Quel que soit le type d'impact, doit être positif pour fonctionner correctement.")]
        public IntCurve impactPower = 1;

        public Explosion explosion;
        
        [Serializable]
        public class Explosion
        {
            [Tooltip("Le nombre de bullets libérés à la disparition.")]
            public IntCurve bulletCount = 0;
    
            [Tooltip("Les bullets issus de la disparition du projectile.\n" +
                     "Si plusieurs sont définis, ils seront choisis au hasard.\n" +
                     "Si aucun n'est défini, les projectiles émis seront des clones de ce projectile.")]
            public BulletData[] bullets;
    
            [Tooltip("L'arc de l'explosion en degré. L'arc est dirigé vers Explosion Direction (voir ci-dessous).")]
            public FloatCurve arc = 360;

            [Tooltip("La direction de l'explosion, au cas où l'arc est à moins de 360 degrés.\n" +
                     "BulletDirection : la direction qu'avait le bullet avant d'exploser.\n" +
                     "Player : vers le joueur.\n" +
                     "Random : aléatoire.")]
            public ExplosionDirection direction = ExplosionDirection.Player;
        }
    }
}