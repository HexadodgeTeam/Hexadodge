using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/Bullets/Bullet")]
public class BulletData : ScriptableObject
{
    public BulletSpawning spawning;
    public BulletMovement movement;
    public BulletProperties properties;
}