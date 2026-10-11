using System;
using UnityEngine;

[Serializable]
public class BulletAnimation
{
    public Sprite sprite;
    public ParticleSystem.MinMaxGradient color = Color.white;

    public BulletAnimationInstance Instance() => new(this);
}

public class BulletAnimationInstance
{
    private GradientInstance _gradientInstance;

    public BulletAnimationInstance(BulletAnimation data)
    {
        _gradientInstance = new GradientInstance(data.color);
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
