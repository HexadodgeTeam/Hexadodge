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
    
    public abstract BulletTransformInstance Instance();
}

public abstract class BulletTransformInstance : PeriodDataInstance
{
    private readonly BulletTransformInstance _subInstance = null;

    protected BulletTransformInstance(BulletTransform data) : base(data)
    {
        if (data.subTransform)
        {
            _subInstance = data.subTransform.Instance();
            
            if (!data.subPeriodOverride.IsAlmostZero)
            {
                _subInstance.OverridePeriodInstance(data.subPeriodOverride.Instance());
            }
        }
    }

    public (Vector2 p, float r, Vector2 s) Transform(Vector2 forward, float lifetime01)
    {
        float period01 = GetPeriod(lifetime01);
        
        (Vector2 p, float r, Vector2 s) transform = TransformTimed(forward, period01);
        (Vector2 p, float r, Vector2 s) subTransform =
            _subInstance?.Transform(forward, lifetime01)
            ?? (Vector2.zero, 0, Vector2.one);
        return (
            p: transform.p + subTransform.p,
            r: transform.r + subTransform.r,
            s: new Vector2(transform.s.x * subTransform.s.x, transform.s.y * subTransform.s.y));
    }

    protected abstract (Vector2 p, float r, Vector2 s) TransformTimed(Vector2 forward, float lifetime01);

    public override void ReRoll()
    {
        base.ReRoll();
        _subInstance?.ReRoll();
    }
}