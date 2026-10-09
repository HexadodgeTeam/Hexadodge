using UnityEngine;

public abstract class PeriodData : ScriptableObject
{
    [Tooltip("Toutes les autres curves sur ce scriptable bouclent sur cette durée en secondes.\n" +
             "/!\\ Cette curve dépend donc de la période du scriptable parent.\n" +
             "Si = 0 : toutes les curves du scriptable dépendent de la durée de vie du scriptable.\n")]
    [SerializeField] internal FloatCurve period;
}

public class PeriodDataInstance
{
    private FloatCurveInstance _periodInstance;
    private float _periodStart;
    private float _currentPeriod;

    protected PeriodDataInstance(PeriodData data)
    {
        _periodInstance = data.period.Instance(0f);
        _currentPeriod = _periodInstance.Eval(0f);
        _periodStart = 0f;
    }

    protected float GetPeriod(float time)
    {
        if (_periodInstance.IsDefault) return time;
        
        float period = time - _periodStart;
        if (period >= _currentPeriod)
        {
            _currentPeriod = _periodInstance.Eval(time);
            _periodStart = time;
        }

        return period;
    }

    public virtual void ReRoll()
    {
        _periodInstance.ReRoll();
        _currentPeriod = _periodInstance.Eval(0f);
        _periodStart = 0f;
    }
}