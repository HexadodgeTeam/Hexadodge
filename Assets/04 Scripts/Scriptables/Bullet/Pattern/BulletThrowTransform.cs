using UnityEngine;

public abstract class ThrowTransform : ScriptableObject
{
    public abstract (Vector2 p, float r) TransformSpawn(float time01, float random01, (Vector2 p, float r) spawn);
}