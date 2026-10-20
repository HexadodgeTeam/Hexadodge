using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemyData : ScriptableObject
{
    public int maxHealth;
    public RuntimeAnimatorController animator;
    [Tooltip("Durée de l'animation de transition vers le prochain combat.")]
    [SerializeField] internal float transitionDuration = 1f;
}