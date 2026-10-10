using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "Scriptable Objects/Bullets/Pattern")]
public class BulletPattern : PeriodData
{
    [Tooltip("Tous les bullets instanciés en un seul jet.")]
    [SerializeField] internal BulletThrow[] bulletThrows;

    [Tooltip("L'intervalle de jet.")]
    [SerializeField] internal FloatCurve throwInterval = 1;

    public BulletPatternInstance Instance() => new(this);
}

public class BulletPatternInstance : PeriodDataInstance
{
    private readonly BulletThrowInstance[] _bulletThrowInstances;
    private FloatCurveInstance _throwIntervalInstance;
    private float _throwTimer = 0f;

    public BulletPatternInstance(BulletPattern data) : base(data)
    {
        _bulletThrowInstances = new BulletThrowInstance[data.bulletThrows.Length];
        
        for (int i = 0; i < data.bulletThrows.Length; i++)
        {
            _bulletThrowInstances[i] = data.bulletThrows[i].Instance();
        }

        _throwIntervalInstance = data.throwInterval.Instance();
    }

    public void GetSpawns(in List<(BulletData b, Vector2 p, float r, (bool x, bool y) mirror)> spawns, float time)
    {
        if (_throwTimer > 0f)
        {
            _throwTimer -= Time.deltaTime;
        }
        else
        {
            foreach (BulletThrowInstance bulletThrow in _bulletThrowInstances)
            {
                (BulletData b, Vector2 p, float r, (bool x, bool y) mirror)? spawn = bulletThrow.GetSpawn(time);
                if (spawn.HasValue)
                    spawns.Add(spawn.Value);
            }
            float period01 = GetPeriod01(time);
            _throwTimer = _throwIntervalInstance.Eval(period01);
        }
    }
}

[Serializable]
public class BulletThrow
{
    [Tooltip("Le bullet.")]
    [SerializeField] internal BulletData bullet;

    [Tooltip("Le spawn du bullet. Peut évoluer selon la période du bullet pattern.")]
    [SerializeField] internal BulletSpawning spawn;
        
    [Tooltip("Inverser le comportement du bullet en X.")]
    [SerializeField] internal bool mirrorX;
        
    [Tooltip("Inverser le comportement du bullet en Y.")]
    [SerializeField] internal bool mirrorY;

    public BulletThrowInstance Instance() => new(this);
}

public class BulletThrowInstance
{
    private readonly BulletThrow _data;

    public BulletThrowInstance(BulletThrow data)
    {
        _data = data;
    }

    public (BulletData b, Vector2 p, float r, (bool x, bool y) mirror)? GetSpawn(float time)
    {
        (Vector2 p, float r)? spawn = _data.spawn.GetSpawn(time, (_data.mirrorX, _data.mirrorY));
        if (spawn.HasValue)
        {
            return (_data.bullet, spawn.Value.p, spawn.Value.r, (_data.mirrorX, _data.mirrorY));
        }

        return null;
    }
}