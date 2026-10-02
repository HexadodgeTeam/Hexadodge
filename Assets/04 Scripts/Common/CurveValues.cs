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

    public FloatCurve(float constant) { curve = constant; }
    public FloatCurve(float min, float max) { curve = new ParticleSystem.MinMaxCurve(min, max); }
    public FloatCurve(ParticleSystem.MinMaxCurve curve) { this.curve = curve; }

    public float Eval(float time, float lerpFactor) => curve.Evaluate(time, lerpFactor);

    public static implicit operator FloatCurve(float constant) => new(constant);
    public static implicit operator FloatCurve(ParticleSystem.MinMaxCurve curve) => new(curve);
    public static implicit operator float(FloatCurve curve) => curve.Random(Random.value);
}

/// <summary>
/// Same as FloatCurve, but locked to and returns int.
/// </summary>
[Serializable]
public struct IntCurve
{
    [SerializeField] private ParticleSystem.MinMaxCurve curve;

    public IntCurve(int constant) { curve = constant; }
    public IntCurve(int min, int max) { curve = new ParticleSystem.MinMaxCurve(min, max); }
    public IntCurve(ParticleSystem.MinMaxCurve curve) { this.curve = curve; }

    public int Eval(float time, float lerpFactor)
    {
        // Special case for TwoConstants mode to be more instinctive.
        if (curve.mode == ParticleSystemCurveMode.TwoConstants)
        {
            int intMin = Mathf.RoundToInt(Mathf.Min(curve.constantMin, curve.constantMax));
            int intMax = Mathf.RoundToInt(Mathf.Max(curve.constantMin, curve.constantMax));
            return Random.Range(intMin, intMax + 1);
        }

        return Mathf.RoundToInt(curve.Evaluate(time, lerpFactor));
    }

    public static implicit operator IntCurve(int constant) => new(constant);
    public static implicit operator IntCurve(ParticleSystem.MinMaxCurve curve) => new(curve);
    public static implicit operator int(IntCurve curve) => curve.Random(Random.value);
}