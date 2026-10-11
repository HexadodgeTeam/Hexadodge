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

    public float Eval(float time01, float lerpFactor) => curve.Evaluate(time01, lerpFactor);

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

    public bool IsAlmostZero => curve.mode == ParticleSystemCurveMode.Constant && curve.constant <= 0.01;

    public (float, float) GetAngleRange() => curve.mode switch
    {
        ParticleSystemCurveMode.Constant => (curve.constant, curve.constant),
        ParticleSystemCurveMode.TwoConstants => (curve.constantMin, curve.constantMax),
        _ => throw new ArgumentOutOfRangeException()
    };

    public static implicit operator FloatCurve(float constant) => new(constant);
    public static implicit operator FloatCurve(ParticleSystem.MinMaxCurve curve) => new(curve);

    public FloatCurveInstance Instance() => new(this);
}

public struct FloatCurveInstance
{
    private FloatCurve _curve;
    private float _random;

    public FloatCurveInstance(FloatCurve curve)
    {
        _curve = curve;
        _random = Random.value;
    }
    
    public float Eval(float time01)
    {
        return _curve.Eval(time01, _random);
    }

    public void ReRoll() => _random = Random.value;

    public bool IsAlmostZero => _curve.IsAlmostZero;
        
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

    public int Eval(float time01, float lerpFactor)
    {
        // Special case for TwoConstants mode to be more instinctive.
        if (curve.mode == ParticleSystemCurveMode.TwoConstants)
        {
            int min = Mathf.RoundToInt(Mathf.Min(curve.constantMin, curve.constantMax));
            int max = Mathf.RoundToInt(Mathf.Max(curve.constantMin, curve.constantMax));
            return Mathf.Min(max, min + Mathf.FloorToInt(Mathf.Clamp01(lerpFactor) * (max - min + 1)));
        }

        return Mathf.RoundToInt(curve.Evaluate(time01, lerpFactor));
    }

    public static implicit operator IntCurve(int constant) => new(constant);
    public static implicit operator IntCurve(ParticleSystem.MinMaxCurve curve) => new(curve);

    public IntCurveInstance Instance() => new(this);
}

public struct IntCurveInstance
{
    private IntCurve _curve;
    private float _random;

    public IntCurveInstance(IntCurve curve)
    {
        _curve = curve;
        _random = Random.value;
    }
    
    public int Eval(float time01)
    {
        return _curve.Eval(time01, _random);
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
    
    public Vector2 Eval(float time01, float lerpFactorX, float lerpFactorY) =>
        new(x.Eval(time01, lerpFactorX), y.Eval(time01, lerpFactorY));
    
    public static implicit operator Vector2Curve(Vector2 vector2) => new(vector2.x, vector2.y);

    public Vector2CurveInstance Instance() => new(this);
}

public struct Vector2CurveInstance
{
    private FloatCurveInstance _curveX;
    private FloatCurveInstance _curveY;
    
    public Vector2CurveInstance(Vector2Curve curve)
    {
        _curveX = curve.x.Instance();
        _curveY = curve.y.Instance();
    }
    
    public Vector2 Eval(float time01) => new(_curveX.Eval(time01), _curveY.Eval(time01));

    public void ReRoll()
    {
        _curveX.ReRoll();
        _curveY.ReRoll();
    }
}

[Serializable]
public struct GradientInstance
{
    private ParticleSystem.MinMaxGradient _gradient;
    private float _random;

    public GradientInstance(ParticleSystem.MinMaxGradient gradient)
    {
        _gradient = gradient;
        _random = Random.value;
    }
    
    public Color Eval(float time01)
    {
        return _gradient.Evaluate(time01, _random);
    }

    public void ReRoll() => _random = Random.value;
        
    public static implicit operator Color(GradientInstance curveInstance) => curveInstance.Eval(1f);
}