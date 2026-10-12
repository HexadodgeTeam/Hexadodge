using UnityEngine;

public abstract class BulletTransform : PeriodData
{
    [Header("Sub Transform")]
    [Tooltip("Un sous-transform qui s'assemblera avec celui-ci. Exemple : ce transform grandit progressivement, " +
             "le sous-transform oscille de taille, le résultat sera une oscillation de taille qui grandit " +
             "progressivement. Peut être vide pour pas de sous-transform.")]
    [SerializeField] internal BulletTransform subTransform;
    
    [Tooltip("Si définie, la période du sous-transform sera override par cette période.")]
    [SerializeField] internal FloatCurve subPeriodOverride = 0f;
    
    public abstract BulletTransformInstance Instance(Vector2 bulletPos);
}

public abstract class BulletTransformInstance : PeriodDataInstance
{
    private readonly BulletTransformInstance _subInstance = null;

    protected BulletTransformInstance(BulletTransform data, Vector2 bulletPos) : base(data)
    {
        if (data.subTransform)
        {
            _subInstance = data.subTransform.Instance(bulletPos);
            
            if (!data.subPeriodOverride.IsAlmostZero)
            {
                _subInstance.OverridePeriodInstance(data.subPeriodOverride.Instance());
            }
        }
    }

    public (Vector2 p, float r, Vector2 s) Transform((bool x, bool y) mirror, Vector2 forward, float lifetime01)
    {
        float period01 = GetPeriod01(lifetime01);
        
        (Vector2 p, float r, Vector2 s) transform = TransformTimed(mirror, forward, period01);
        (Vector2 p, float r, Vector2 s) subTransform =
            _subInstance?.Transform(mirror, forward, lifetime01) ?? (Vector2.zero, 0, Vector2.one);
        
        return (
            p: transform.p + subTransform.p,
            r: transform.r + subTransform.r,
            s: new Vector2(transform.s.x * subTransform.s.x, transform.s.y * subTransform.s.y));
    }
    
    protected abstract (Vector2 p, float r, Vector2 s) TransformTimed((bool x, bool y) mirror, Vector2 forward, float lifetime01);

    public virtual void ReRollTransform(Vector2 bulletPos)
    {
        ReRoll();
        _subInstance?.ReRoll();
    }
}