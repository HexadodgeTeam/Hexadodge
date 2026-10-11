using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Fight", menuName = "Scriptable Objects/Fight/Data")]
public class FightData : ScriptableObject, IGizmosDrawer
{
    [Tooltip("Les différent paliers du combat, contenant chacun une phase.")]
    [SerializeField] internal FightLevel[] levels;

    [Tooltip("Les données de l'ennemi.")]
    [SerializeField] internal EnemyData enemy;
    
    public FightDataInstance Instance(Action<BulletSpawn> spawnAction) => new(this, spawnAction);
    
    public void OnDrawGizmos()
    {
        foreach (FightLevel level in levels)
        {
            level.phase.OnDrawGizmos();
        }
    }
}

public class FightDataInstance
{
    private int _currentLevelIndex;
    private readonly FightLevelInstance[] _levels;
    
    public FightDataInstance(FightData data, Action<BulletSpawn> spawnAction)
    {
        _levels = new FightLevelInstance[data.levels.Length];
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i] = data.levels[i].Instance(spawnAction);
        }
        _currentLevelIndex = GetLevelIndex(1);
        _levels[_currentLevelIndex].phase.Start();
    }

    public void Tick(float enemyHealthFraction)
    {
        int nextLevelIndex = GetLevelIndex(enemyHealthFraction);

        if (_currentLevelIndex != nextLevelIndex)
        {
            _currentLevelIndex = nextLevelIndex;
            _levels[_currentLevelIndex].phase.Start();
        }
        
        _levels[nextLevelIndex].phase.Tick();
    }

    private int GetLevelIndex(float enemyHealthRatio)
    {
        int levelIndex = -1;
        float lowestValidRatio = 2f;
        
        for (var i = 0; i < _levels.Length; i++)
        {
            if (enemyHealthRatio <= _levels[i].enemyHealthFraction && lowestValidRatio > _levels[i].enemyHealthFraction)
            {
                levelIndex = i;
                lowestValidRatio = _levels[i].enemyHealthFraction;
            }
        }
        
        return levelIndex;
    }

    public void ReRoll()
    {
        foreach (FightLevelInstance level in _levels)
        {
            level.phase.ReRoll();
        }
    }
}

[Serializable]
public struct FightLevel
{
    [Tooltip("Phase.")]
    public FightPhase phase;

    [Tooltip("Fraction de la vie de l'ennemi auquel la phase se déclenche.")]
    [Range(0, 1)] public float enemyHealthFraction;

    public FightLevelInstance Instance(Action<BulletSpawn> spawnAction) => new(this, spawnAction);
}

public struct FightLevelInstance
{
    public readonly FightPhaseInstance phase;
    public readonly float enemyHealthFraction;

    public FightLevelInstance(FightLevel data, Action<BulletSpawn> spawnAction)
    {
        phase = data.phase.Instance(spawnAction);
        enemyHealthFraction = data.enemyHealthFraction;
    }
}