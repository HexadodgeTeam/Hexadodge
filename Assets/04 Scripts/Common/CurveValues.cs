using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Wrapper around ParticleSystem.MinMaxCurve that returns floats.
/// </summary>
[Serializable]
public struct FloatCurve
{
    [SerializeField] private ParticleSystem.MinMaxCurve curve;

    public FloatCurve(float constant)
    {
        curve = constant;
        curve.curveMultiplier = constant;
    }
    public FloatCurve(float min, float max) { curve = new ParticleSystem.MinMaxCurve(min, max); }
    public FloatCurve(ParticleSystem.MinMaxCurve curve) { this.curve = curve; }

    public float Eval(float time, float lerpFactor) => curve.Evaluate(time, lerpFactor);

    public bool CanGizmo => curve.mode is ParticleSystemCurveMode.Constant or ParticleSystemCurveMode.TwoConstants;
    
    public float Center => curve.mode switch
    {
        ParticleSystemCurveMode.Constant => curve.constant,
        ParticleSystemCurveMode.TwoConstants => (curve.constantMin + curve.constantMax) / 2,
        _ => throw new ArgumentOutOfRangeException()
    };
    public float Range => curve.mode switch
    {
        ParticleSystemCurveMode.Constant => 0f,
        ParticleSystemCurveMode.TwoConstants => Mathf.Abs(curve.constantMax - curve.constantMin),
        _ => throw new ArgumentOutOfRangeException()
    };
    public bool IsDefault(float defaultValue)
    {
        return curve.mode == ParticleSystemCurveMode.Constant && curve.constant == defaultValue;
    }
    public float Default => curve.constant;
    public bool ParentPeriod => curve.mode == ParticleSystemCurveMode.Constant && curve.constant == -1f;

    public (float, float) GetAngleRange() => curve.mode switch
    {
        ParticleSystemCurveMode.Constant => (curve.constant, curve.constant),
        ParticleSystemCurveMode.TwoConstants => (curve.constantMin, curve.constantMax),
        _ => throw new ArgumentOutOfRangeException()
    };

    public static implicit operator FloatCurve(float constant) => new(constant);
    public static implicit operator FloatCurve(ParticleSystem.MinMaxCurve curve) => new(curve);

    public FloatCurveInstance Instance(float defaultValue) => new(this, defaultValue);
}

public struct FloatCurveInstance
{
    private const float Default = -1f;
    private FloatCurve _curve;
    private float _random;

    public FloatCurveInstance(FloatCurve curve, float defaultValue)
    {
        _curve = curve;
        _random = curve.IsDefault(defaultValue) ? Default : Random.value;
    }

    
    public float Eval(float time)
    {
        return IsDefault ? _curve.Default : _curve.Eval(time, _random);
    }
    public bool IsDefault => _random == Default;
    public bool ParentPeriod => _curve.ParentPeriod;

    public void ReRoll() => _random = Random.value;
        
    public static implicit operator float(FloatCurveInstance curveInstance) => curveInstance.Eval(1f);
}

/// <summary>
/// Same as FloatCurve, but locked to and returns int.
/// </summary>
[Serializable]
public struct IntCurve
{
    [SerializeField] private ParticleSystem.MinMaxCurve curve;

    public IntCurve(int constant)
    {
        curve = constant;
        curve.curveMultiplier = constant;
    }
    public IntCurve(int min, int max) { curve = new ParticleSystem.MinMaxCurve(min, max); }
    public IntCurve(ParticleSystem.MinMaxCurve curve) { this.curve = curve; }

    public int Eval(float time, float lerpFactor)
    {
        // Special case for TwoConstants mode to be more instinctive.
        if (curve.mode == ParticleSystemCurveMode.TwoConstants)
        {
            int min = Mathf.RoundToInt(Mathf.Min(curve.constantMin, curve.constantMax));
            int max = Mathf.RoundToInt(Mathf.Max(curve.constantMin, curve.constantMax));
            return Mathf.Min(max, min + Mathf.FloorToInt(Mathf.Clamp01(lerpFactor) * (max - min + 1)));
        }

        return Mathf.RoundToInt(curve.Evaluate(time, lerpFactor));
    }
    public bool IsDefault(float defaultValue)
    {
        return curve.mode == ParticleSystemCurveMode.Constant && curve.constant == defaultValue;
    }
    public int Default => Mathf.RoundToInt(curve.constant);

    public static implicit operator IntCurve(int constant) => new(constant);
    public static implicit operator IntCurve(ParticleSystem.MinMaxCurve curve) => new(curve);

    public IntCurveInstance Instance(float defaultValue) => new(this, defaultValue);
}

public struct IntCurveInstance
{
    private const float Default = -1f;
    private IntCurve _curve;
    private float _random;

    public IntCurveInstance(IntCurve curve, float defaultValue)
    {
        _curve = curve;
        _random = curve.IsDefault(defaultValue) ? Default : Random.value;
    }
    
    public bool IsDefault => _random == Default;
    
    public int Eval(float time)
    {
        return IsDefault ? _curve.Default : _curve.Eval(time, _random);
    }

    public void ReRoll() => _random = Random.value;
    
    public static implicit operator int(IntCurveInstance curveInstance) => curveInstance.Eval(1f);
}

[Serializable]
public struct Vector2Curve
{
    public FloatCurve x;
    public FloatCurve y;

    public Vector2Curve(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    
    public Rect? GetGizmosRect()
    {
        if (!x.CanGizmo || !y.CanGizmo) return null;

        float width = x.Range;
        float height = y.Range;
        Rect rect = new(x.Center - width / 2f, y.Center - height / 2f, width, height);
        
        return rect;
    }
    
    public Vector2 Eval(float time, float lerpFactorX, float lerpFactorY) =>
        new(x.Eval(time, lerpFactorX), y.Eval(time, lerpFactorY));
    
    public static implicit operator Vector2Curve(Vector2 vector2) => new(vector2.x, vector2.y);

    public Vector2CurveInstance Instance(float defaultX, float defaultY) => new(this, defaultX, defaultY);
}

public struct Vector2CurveInstance
{
    private FloatCurveInstance _curveX;
    private FloatCurveInstance _curveY;
    
    public Vector2CurveInstance(Vector2Curve curve, float defaultX, float defaultY)
    {
        _curveX = curve.x.Instance(defaultX);
        _curveY = curve.y.Instance(defaultY);
    }
    
    public bool IsDefault => _curveX.IsDefault && _curveY.IsDefault;
    
    public Vector2 Eval(float time) => new(_curveX.Eval(time), _curveY.Eval(time));

    public void ReRoll()
    {
        _curveX.ReRoll();
        _curveY.ReRoll();
    }
}