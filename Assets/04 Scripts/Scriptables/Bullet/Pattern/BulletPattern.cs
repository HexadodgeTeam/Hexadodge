using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "Scriptable Objects/Bullets/Pattern")]
public class BulletPattern : PeriodData, IGizmosDrawer
{
    [Tooltip("Tous les bullets instanciés en un seul jet.")]
    [SerializeField]
    internal BulletThrow[] bulletThrows;

    [Tooltip("L'intervalle de jet.")]
    [SerializeField]
    internal FloatCurve throwInterval = 1;

    public BulletPatternInstance Instance(Action<BulletSpawn> spawnAction) => new(this, spawnAction);

    public void OnDrawGizmos()
    {
        foreach (BulletThrow bulletThrow in bulletThrows)
        {
            bulletThrow.spawn?.OnDrawGizmos();
        }
    }
}

public class BulletPatternInstance : PeriodDataInstance
{
    private readonly BulletThrowInstance[] _bulletThrowInstances;
    private FloatCurveInstance _throwIntervalInstance;
    private float _throwTimer = 0f;

    public BulletPatternInstance(BulletPattern data, Action<BulletSpawn> spawnAction) : base(data)
    {
        _bulletThrowInstances = new BulletThrowInstance[data.bulletThrows.Length];
        for (int i = 0; i < _bulletThrowInstances.Length; i++)
        {
            _bulletThrowInstances[i] = data.bulletThrows[i].Instance(spawnAction);
        }
        
        _throwIntervalInstance = data.throwInterval.Instance();
    }

    public void Tick(float time01)
    {
        if (_throwTimer > 0f)
        {
            _throwTimer -= Time.deltaTime;
        }
        else
        {
            float period01 = GetPeriod01(time01);
            
            foreach (BulletThrowInstance bulletThrow in _bulletThrowInstances)
            {
                bulletThrow.SpawnBullet(period01);
            }
            
            _throwTimer = _throwIntervalInstance.Eval(period01);
        }
    }

    public override void ReRoll()
    {
        base.ReRoll();
        
        foreach (BulletThrowInstance bulletThrow in _bulletThrowInstances)
        {
            bulletThrow.ReRoll();
        }
        _throwIntervalInstance.ReRoll();
        _throwTimer = 0f;
    }
}

[Serializable]
public class BulletThrow
{
    [Tooltip("Le bullet.")]
    [SerializeField]
    internal BulletData bullet;

    [Tooltip("Le spawn du bullet. Peut évoluer selon la période du bullet pattern.")]
    [SerializeField]
    internal BulletSpawning spawn;

    [Tooltip("Nombre de bullets à faire apparaître.")]
    [SerializeField]
    internal IntCurve spawnCount = 1;
    
    [Tooltip("Si défini, la période utilisée pour le spawn est celle-ci au lieu de la période du pattern.")]
    [SerializeField]
    internal FloatCurve spawnPeriodOverride;
    
    [Tooltip("Inverser le comportement du bullet en X.")]
    [SerializeField]
    internal bool mirrorX;
        
    [Tooltip("Inverser le comportement du bullet en Y.")]
    [SerializeField]
    internal bool mirrorY;

    public BulletThrowInstance Instance(Action<BulletSpawn> spawnAction) => new(this, spawnAction);
}

public class BulletThrowInstance
{
    private readonly BulletThrow _data;
    private readonly Action<BulletSpawn> _spawnAction;
    private PeriodTimer _periodTimer;
    private IntCurveInstance _spawnCount;
    
    public BulletThrowInstance(BulletThrow data, Action<BulletSpawn> spawnAction)
    {
        _data = data;
        _periodTimer = new PeriodTimer(_data.spawnPeriodOverride);
        _spawnAction = spawnAction;
        _spawnCount = data.spawnCount.Instance();
    }

    public void SpawnBullet(float time01)
    {
        float period01 = _data.spawnPeriodOverride.IsAlmostZero ? time01 : _periodTimer.GetPeriod01(time01);
        int spawnCount = _spawnCount.Eval(period01);
        if (spawnCount == 0) spawnCount = 1;

        for (int i = 0; i < spawnCount; i++)
        {
            float count01 = spawnCount == 1 ? 0.5f : i / (float)(spawnCount - 1);
            (Vector2 p, float r) s = _data.spawn.GetCombinedSpawn(
                period01,
                (_data.mirrorX, _data.mirrorY),
                count01);
            
            // Spawn bullet.
            _spawnAction.Invoke(new BulletSpawn(_data.bullet, s.p, s.r, (_data.mirrorX, _data.mirrorY)));
        }
    }

    public void ReRoll()
    {
        _periodTimer.ReRoll();
    }
}