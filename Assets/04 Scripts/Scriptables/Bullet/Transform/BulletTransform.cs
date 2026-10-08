using UnityEngine;

public abstract class BulletTransform : ScriptableObject
{
    [Tooltip("Toutes les curves bouclent sur cette période.")]
    [SerializeField] internal FloatCurve period;
    
    [Header("Sub Transform")]
    [Tooltip("Un sous-transform qui s'assemblera avec celui-ci. Exemple : ce transform grandit progressivement, " +
             "le sous-transform oscille de taille, le résultat sera une oscillation de taille qui grandit " +
             "progressivement. Peut être vide pour pas de sous-transform.")]
    [SerializeField] internal BulletTransform subTransform;

    [Header("À APPLIQUER AU TRANSFORM ENFANT")]
    [Tooltip("OBSOLÈTE. Appliquer cette valeur au champ period du transform enfant.")]
    [SerializeField] internal FloatCurve subTransformPeriod;
    
    public abstract BulletTransformInstance Instance();
}

public abstract class BulletTransformInstance
{
    private readonly BulletTransformInstance _subInstance;
    private FloatCurveInstance _periodInstance;
    private float _periodStart;
    private float _currentPeriod;

    protected BulletTransformInstance(BulletTransform data)
    {
        _subInstance = data.subTransform ? data.subTransform.Instance() : null;
        _periodInstance = data.period.Instance();
        _currentPeriod = _periodInstance.Eval(0f);
        _periodStart = 0f;
    }

    public (Vector2 p, float r, Vector2 s) Transform(Vector2 forward, float time)
    {
        float periodTime = time - _periodStart;
        if (periodTime >= _currentPeriod)
        {
            _currentPeriod = _periodInstance.Eval(time);
            _periodStart = time;
        }

        (Vector2 p, float r, Vector2 s) transform = TransformTimed(forward, periodTime / _currentPeriod);
        (Vector2 p, float r, Vector2 s) subTransform =
            _subInstance?.Transform(forward, time) ?? (Vector2.zero, 0, Vector2.one);
        Debug.Log("sda " + transform.s);
        Debug.Log("srwqr " + subTransform.s);
        return (
            p: transform.p + subTransform.p,
            r: transform.r + subTransform.r,
            s: new Vector2(transform.s.x * subTransform.s.x, transform.s.y * subTransform.s.y));
    }

    protected abstract (Vector2 p, float r, Vector2 s) TransformTimed(Vector2 forward, float time);

    public virtual void ReRoll()
    {
        _subInstance?.ReRoll();
        _periodInstance.ReRoll();
        _currentPeriod = _periodInstance.Eval(0f);
        _periodStart = 0f;
    }
}