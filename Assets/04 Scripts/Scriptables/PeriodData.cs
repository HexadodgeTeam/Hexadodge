using UnityEngine;

public abstract class PeriodData : ScriptableObject
{
    [Tooltip("Toutes les autres curves sur ce scriptable bouclent sur cette durée en secondes.\n" +
             "/!\\ Cette curve dépend donc de la durée de vie du scriptable.\n" +
             "Si = 0 : toutes les curves du scriptable dépendent de la durée de vie du scriptable.\n")]
    [SerializeField]
    internal FloatCurve period;
}

public class PeriodDataInstance
{
    private PeriodTimer _periodTimer;

    protected PeriodDataInstance(PeriodData data)
    {
        _periodTimer = new PeriodTimer(data.period);
    }

    protected void OverridePeriodInstance(FloatCurveInstance periodInstance) =>
        _periodTimer.OverridePeriodInstance(periodInstance);

    protected float GetPeriod01(float lifetime01) => _periodTimer.GetPeriod01(lifetime01);
    
    public virtual void ReRoll() => _periodTimer.ReRoll();
}

public struct PeriodTimer
{
    private readonly bool _getLifetimeInsteadOfPeriod;
    private FloatCurveInstance _periodInstance;
    private float _periodStart;
    private float _currentPeriod;

    public PeriodTimer(FloatCurve period)
    {
        _periodInstance = period.Instance();
        _getLifetimeInsteadOfPeriod = _periodInstance.IsAlmostZero;
        _currentPeriod = !_getLifetimeInsteadOfPeriod ? _periodInstance.Eval(0f) : 0f;
        _periodStart = Game.ScaledTime;
    }

    public void OverridePeriodInstance(FloatCurveInstance periodInstance)
    {
        _periodInstance = periodInstance;
    }

    public float GetPeriod01(float lifetime01)
    {
        if (_getLifetimeInsteadOfPeriod) return lifetime01;

        float period = Game.ScaledTime - _periodStart;
        if (period >= _currentPeriod)
        {
            _currentPeriod = _periodInstance.Eval(lifetime01);
            _periodStart = Game.ScaledTime;
            period = 0f;
        }

        return period / _currentPeriod;
    }

    public void ReRoll()
    {
        _periodInstance.ReRoll();
        _currentPeriod = _periodInstance.Eval(0f);
        _periodStart = 0f;
    }
}