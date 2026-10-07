using UnityEngine;

public abstract class BulletTransform : ScriptableObject
{
    [Header("Sub Transform")]
    [Tooltip("Un sous-transform qui s'assemblera avec celui-ci. Exemple : ce transform grandit progressivement, " +
             "le sous-transform oscille de taille, le résultat sera une oscillation de taille qui grandit " +
             "progressivement. Peut être vide pour pas de sous-transform.")]
    [SerializeField] internal BulletTransform subTransform;

    [Tooltip("La période du sous-transform. Cette période s'appliquera au(x) courbe(s) du sous-transform.\n" +
             "Exemple : une période de 2.0 avec un sous-transform qui oscille de taille = un transform qui oscille " +
             "toutes les 2 secondes.\n" +
             "!! Attention !! : la période du sous-transform doit être inférieure à la période du transform pour " +
             "fonctionner correctement.")]
    [SerializeField] internal FloatCurve subTransformPeriod;
    
    public abstract BulletTransformInstance Instance();
}

public abstract class BulletTransformInstance
{
    private readonly BulletTransformInstance _subInstance;
    private FloatCurveInstance _subInstancePeriod;
    private float _periodStart;
    private float _currentPeriod;

    protected BulletTransformInstance(BulletTransform data)
    {
        _subInstance = data.subTransform ? data.subTransform.Instance() : null;
        _subInstancePeriod = data.subTransformPeriod.Instance();
        _currentPeriod = _subInstancePeriod.Eval(0f);
        _periodStart = 0f;
    }

    public virtual (Vector2, float, Vector2) Transform(Vector2 forward, float time, float realTime)
    {
        if (_subInstance == null) return (Vector2.zero, 0, Vector2.one);
        
        float periodTime = realTime - _periodStart;
        if (periodTime >= _currentPeriod)
        {
            _currentPeriod = _subInstancePeriod.Eval(time);
            _periodStart = realTime;
        }

        return _subInstance.Transform(forward, periodTime / _currentPeriod, realTime);
    }

    public virtual void ReRoll()
    {
        _subInstance?.ReRoll();
        _subInstancePeriod.ReRoll();
        _currentPeriod = _subInstancePeriod.Eval(0f);
        _periodStart = 0f;
    }
}