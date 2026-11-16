using System;
using UnityEngine;

[Serializable]
public class BulletAnimation
{
    public Sprite sprite;
    public bool enableSpriteOverLifetime;
    public Sprite[] spriteOverLifetime;
    public IntCurve spriteOverLifetimeCurve;
    public ParticleSystem.MinMaxGradient color = Color.white;

    public BulletAnimationInstance Instance() => new(this);
}

public class BulletAnimationInstance
{
    private BulletAnimation _data;
    private GradientInstance _gradientInstance;
    private IntCurveInstance _spriteOverLifetimeInstance;

    public BulletAnimationInstance(BulletAnimation data)
    {
        _data = data;
        _gradientInstance = new GradientInstance(data.color);
        if (data.enableSpriteOverLifetime) _spriteOverLifetimeInstance = data.spriteOverLifetimeCurve.Instance();
    }

    public Sprite GetSpriteOverLifetime(float lifetime01)
    {
        int index = _spriteOverLifetimeInstance.Eval(lifetime01);

        if (index >= 0 && index < _data.spriteOverLifetime.Length)
            return _data.spriteOverLifetime[_spriteOverLifetimeInstance.Eval(lifetime01)];
        
        return null;
    }
    
    public Color GetColor(float lifetime01)
    {
        return _gradientInstance.Eval(lifetime01);
    }

    public void ReRoll()
    {
        _gradientInstance.ReRoll();
    }
}
