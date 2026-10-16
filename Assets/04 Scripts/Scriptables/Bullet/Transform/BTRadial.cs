using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Radial", menuName = "Scriptable Objects/Bullets/Transform/Radial")]
public class BTRadial : BulletTransform
{
    internal enum PoleReference
    {
        World,
        BulletRelative,
        PlayerRelative
    }
    
    [Header("Base")]
    [Tooltip("Position/offset initial du pôle selon le référentiel (voir variable suivante).\n" +
             "Utiliser une curve ici n'a aucun effet.")]
    [SerializeField]
    internal Vector2Curve basePolePosition;

    [Tooltip("Référentiel du positionnement du pôle.\n" +
             "World : coordonnées du monde (par rapport au (0, 0)).\n" +
             "BulletRelative : coordonnées relatives à la position du bullet.\n" +
             "PlayerRelative : coordonnées relatives à la position du joueur.")]
    [SerializeField]
    internal PoleReference basePolePositionReference = PoleReference.BulletRelative;
    
    [Header("Movement")]
    [Tooltip("Vitesse de déplacement du pôle (centre du cercle).")]
    [SerializeField]
    internal Vector2Curve poleSpeed;

    [Tooltip("Vitesse de déplacement du bullet par rapport au pôle (>0 = s'éloigne).")]
    [SerializeField]
    internal FloatCurve distanceSpeed;
    
    [Tooltip("Vitesse angulaire par rapport à l'angle actuel (>0 = anti-horaire).")]
    [SerializeField]
    internal FloatCurve angleSpeed;
    
    public override BulletTransformInstance Instance(Vector2 bulletPos) => new BTRadialInstance(this, bulletPos);
}

public class BTRadialInstance : BulletTransformInstance
{
    private readonly BTRadial _data;
    private Vector2CurveInstance _poleSpeed;
    private FloatCurveInstance _distanceSpeed;
    private FloatCurveInstance _angleSpeed;
    
    private Vector2 _pole;
    private float _angle;
    private float _distance;
    private Vector2 _lastPos;

    public BTRadialInstance(BTRadial data, Vector2 bulletPos) : base(data, bulletPos)
    {
        _data = data;
        _poleSpeed = data.poleSpeed.Instance();
        _distanceSpeed = data.distanceSpeed.Instance();
        _angleSpeed = data.angleSpeed.Instance();
        _lastPos = bulletPos;
        
        Start(bulletPos);
    }
    
    protected override (Vector2 p, float r, Vector2 s) TransformTimed((bool x, bool y) mirror, Vector2 forward, float lifetime01)
    {
        _pole += _poleSpeed.Eval(lifetime01) * Time.fixedDeltaTime;
        _angle += _angleSpeed.Eval(lifetime01) * Time.fixedDeltaTime;
        _distance += _distanceSpeed.Eval(lifetime01) * Time.fixedDeltaTime;
        
        float rad = _angle * Mathf.Deg2Rad;
        Vector2 position = _pole + new Vector2(
            _distance * Mathf.Cos(rad) * (mirror.x ? -1 : 1), 
            _distance * Mathf.Sin(rad) * (mirror.y ? -1 : 1));
        
        Vector2 move = position - _lastPos;
        _lastPos = position;

        return (move, 0f, Vector2.one);
    }

    public override void ReRollTransform(Vector2 bulletPos)
    {
        base.ReRoll();
        _poleSpeed.ReRoll();
        _distanceSpeed.ReRoll();
        _angleSpeed.ReRoll();
        
        Start(bulletPos);
    }

    private void Start(Vector2 bulletPos)
    {
        _pole = _data.basePolePosition.Eval(1f, Random.value, Random.value);
        _pole += _data.basePolePositionReference switch
        {
            BTRadial.PoleReference.World => new Vector2(0, 0),
            BTRadial.PoleReference.BulletRelative => bulletPos,
            BTRadial.PoleReference.PlayerRelative => Player.Pos,
            _ => throw new ArgumentOutOfRangeException()
        };
        Vector2 bulletToPole = _pole - bulletPos;
        _angle = Vector2.SignedAngle(Vector2.right, bulletToPole);
        _distance = Vector2.Distance(_pole, bulletPos);
    }
}