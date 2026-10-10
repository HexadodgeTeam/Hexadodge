using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/Bullets/Bullet")]
public class BulletData : ScriptableObject
{
    public BulletTransform transform;
    public BulletProperties properties;
    public BulletAnimation animation;
}

[Serializable]
public class BulletAnimation
{
    public Sprite sprite;
}