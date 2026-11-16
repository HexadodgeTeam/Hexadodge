using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TurnToPlayer", menuName = "Scriptable Objects/Bullets/Transform/TurnToPlayer")]
public class BTTurnToTarget : BulletTransform
{
    internal enum TargetType
    {
        WorldPos,
        Player
    }

    [Header("Turn To Target")]
    [Tooltip("Référentiel de la cible vers laquelle se tourner.\n" +
             "WorldPos : coordonnées (0, 0) du monde (modifiable avec l'offset).\n" +
             "Player : coordonnées du joueur.")]
    [SerializeField]
    internal TargetType targetType = TargetType.Player;

    [Tooltip("Offset par rapport à la au référentiel à cibler. Surtout utile pour le mode WorldPos.")]
    [SerializeField]
    internal Vector2Curve targetOffset;

    [Tooltip("Vitesse de rotation vers la cible.")]
    [SerializeField]
    internal FloatCurve rotationSpeed;
    
    public override BulletTransformInstance Instance(Vector2 bulletPos) => new BTTurnToTargetInstance(this, bulletPos);
}

public class BTTurnToTargetInstance : BulletTransformInstance
{
    private BTTurnToTarget _data;
    private Vector2CurveInstance _targetOffset;
    private FloatCurveInstance _rotationSpeed;

    public BTTurnToTargetInstance(BTTurnToTarget data, Vector2 bulletPos) : base(data, bulletPos)
    {
        _data = data;
        _targetOffset = data.targetOffset.Instance();
        _rotationSpeed = data.rotationSpeed.Instance();
    }
    
    protected override (Vector2 p, float r, Vector2 s) TransformTimed((bool x, bool y) mirror, Vector2 forward,
        float lifetime01, Vector2 pos)
    {
        Vector2 target = _targetOffset.Eval(lifetime01) + _data.targetType switch
        {
            BTTurnToTarget.TargetType.WorldPos => new Vector2(0, 0),
            BTTurnToTarget.TargetType.Player => Player.Pos,
            _ => throw new ArgumentOutOfRangeException()
        };

        Vector2 bulletToTarget = target - pos;
        
        float signedAngle = Vector2.SignedAngle(forward, bulletToTarget);
        float rotation = _rotationSpeed.Eval(lifetime01) * Game.DeltaTime;
        if (signedAngle < 0) rotation *= -1;

        return (Vector2.zero, rotation, Vector2.one);
    }

    public override void ReRollTransform(Vector2 bulletPos)
    {
        base.ReRoll();
    }
}