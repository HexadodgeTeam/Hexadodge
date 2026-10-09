using UnityEngine;

[CreateAssetMenu(fileName = "General", menuName = "Scriptable Objects/Bullets/Transform/General")]
public class BTGeneral : BulletTransform
{
    [Header("Position")]
    [Tooltip("Vitesse relative à la direction actuelle du bullet (représentée par un trait bleu en debug).\n" +
             "X > 0 : avant.\n" +
             "X < 0 : arrière.\n" +
             "Y > 0 : droite du bullet.\n" +
             "Y < 0 : gauche du bullet.")]
    [SerializeField] internal Vector2Curve forwardSpeed;

    [Tooltip("Vitesse absolue par rapport au monde. Exemple : X > 0 = vers la droite.")]
    [SerializeField] internal Vector2Curve worldSpeed;
    
    [Tooltip("Le bullet suit le mouvement du joueur par ce facteur.\n" +
             "0 : le bullet est totalement indépendant de la position du joueur.\n" +
             "0.5 : le bullet suit le joueur de moitié.\n" +
             "1 : le bullet effectue exactement les mêmes déplacements que le joueur.")]
    [SerializeField] internal FloatCurve playerFollowMultiplier = 0f;
    
    [Header("Rotation")]
    [Tooltip("Vitesse de rotation.\n" +
             "0 : ne change pas de rotation.\n" +
             "< 0 : tourne dans le sens horaire.\n" +
             "> 0 : tourne dans le sens anti-horaire.")]
    [SerializeField] internal FloatCurve rotationSpeed;
    
    [Header("Scale")]
    [Tooltip("Taille du bullet. 0.25 par défaut. Par rapport à la direction du bullet :\n" +
             "X : Scale avant-arrière.\n" +
             "Y : Scale gauche-droite.")]
    [SerializeField] internal Vector2Curve scale = new(1, 1);
    [Tooltip("Multiplicateur simple appliqué à la scale totale, si besoin de contrôler juste la scale globale " +
             "au lieu des composants x et y individuels.")]
    [SerializeField] internal FloatCurve scaleMultiplier = 1f;
    public override BulletTransformInstance Instance() => new BTGeneralInstance(this);
}

public class BTGeneralInstance : BulletTransformInstance
{
    private Vector2CurveInstance _forwardSpeed;
    private Vector2CurveInstance _worldSpeed;
    private FloatCurveInstance _playerFollowMultiplier;
    private FloatCurveInstance _rotationSpeed;
    private Vector2CurveInstance _scale;
    private FloatCurveInstance _scaleMultiplier;

    public BTGeneralInstance(BTGeneral data) : base(data)
    {
        _forwardSpeed = data.forwardSpeed.Instance(0, 0);
        _worldSpeed = data.worldSpeed.Instance(0, 0);
        _playerFollowMultiplier = data.playerFollowMultiplier.Instance(0);
        _rotationSpeed = data.rotationSpeed.Instance(0);
        _scale = data.scale.Instance(1, 1);
        _scaleMultiplier = data.scaleMultiplier.Instance(1);
    }

    protected override (Vector2, float, Vector2) TransformTimed(Vector2 forward, float time)
    {
        Vector2 p = Vector2.zero; // Position
        float r = !_rotationSpeed.IsDefault ? _rotationSpeed.Eval(time) * Time.fixedDeltaTime : 0f; // Rotation
        Vector2 s = !(_scale.IsDefault && _scaleMultiplier.IsDefault) 
            ? _scale.Eval(time) * _scaleMultiplier.Eval(time) 
            : Vector2.one; // Scale

        if (!_forwardSpeed.IsDefault)
        {
            Vector2 right = new(forward.y, forward.x);
            Vector2 velocity = _forwardSpeed.Eval(time);
            p += forward * (velocity.x * Time.fixedDeltaTime) + right * (velocity.y * Time.fixedDeltaTime);
        }
        if (!_worldSpeed.IsDefault)
            p += _worldSpeed.Eval(time) * Time.fixedDeltaTime;
        if (!_playerFollowMultiplier.IsDefault) 
            p += Player.Delta * _playerFollowMultiplier.Eval(time);
        
        return (p, r, s);
    }

    public override void ReRoll()
    {
        base.ReRoll();
        _forwardSpeed.ReRoll();
        _worldSpeed.ReRoll();
        _playerFollowMultiplier.ReRoll();
        _rotationSpeed.ReRoll();
        _scale.ReRoll();
        _scaleMultiplier.ReRoll();
    }
}