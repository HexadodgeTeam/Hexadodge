using System.Collections;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    
    [SerializeField] private Player player;
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private AbstractFight fight;
    [SerializeField] private float paintExplosionDelay = 0.73f;

    private void Awake()
    {
        Instance = this;
    }

    public void MultiplyFillRange(float multiplier)
    {
        player.FillRangeMultiplier *= multiplier;
    }
    
    public void AddAttackBonus(int value)
    {
        fight.AddAttackBonus(value);
    }

    public void ClearBullets()
    {
        bulletSpawner.ClearBullets();
    }

    public void PaintExplosion(float radius)
    {
        StartCoroutine(PaintExplosionRoutine(radius));
    }

    private IEnumerator PaintExplosionRoutine(float radius)
    {
        yield return new WaitForSeconds(paintExplosionDelay);
        
        player.PaintCircle(radius);
    }
}