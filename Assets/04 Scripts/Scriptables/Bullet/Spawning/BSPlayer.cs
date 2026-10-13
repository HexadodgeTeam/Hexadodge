using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Bullets/Spawning/Player")]
public class BSPlayer : BulletSpawning
{
    protected override (Vector2 p, float r) GetSpawn(float time01, (bool x, bool y) mirror, float count01)
    {
        return (Player.Pos, 0f);
    }

    public override void OnDrawGizmos()
    {
    }
}