using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/Bullets/Bullet")]
public class BulletData : ScriptableObject
{
    public BulletSpawning spawning;
    public BulletTransform transform;
    public BulletProperties properties;
}