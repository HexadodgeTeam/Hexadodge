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
    [Tooltip("Désactiver le calcul (optimisation).")]
    [SerializeField] internal bool disableForwardSpeed = false;

    [Tooltip("Vitesse absolue par rapport au monde. Exemple : X > 0 = vers la droite.")]
    [SerializeField] internal Vector2Curve worldSpeed;
    [Tooltip("Désactiver le calcul (optimisation).")]
    [SerializeField] internal bool disableWorldSpeed = false;
    
    [Tooltip("Le bullet suit le mouvement du joueur par ce facteur.\n" +
             "0 : le bullet est totalement indépendant de la position du joueur.\n" +
             "0.5 : le bullet suit le joueur de moitié.\n" +
             "1 : le bullet effectue exactement les mêmes déplacements que le joueur.")]
    [SerializeField] internal FloatCurve playerFollowMultiplier = 0f;
    [Tooltip("Désactiver le calcul (optimisation).")]
    [SerializeField] internal bool disablePFM = false;
    
    [Header("Rotation")]
    [Tooltip("Vitesse de rotation.\n" +
             "0 : ne change pas de rotation.\n" +
             "< 0 : tourne dans le sens horaire.\n" +
             "> 0 : tourne dans le sens anti-horaire.")]
    [SerializeField] internal FloatCurve rotationSpeed;
    [Tooltip("Désactiver le calcul (optimisation).")]
    [SerializeField] internal bool disableRotationSpeed = false;
    
    [Header("Scale")]
    [Tooltip("Taille du bullet. 0.25 par défaut. Par rapport à la direction du bullet :\n" +
             "X : Scale avant-arrière.\n" +
             "Y : Scale gauche-droite.")]
    [SerializeField] internal Vector2Curve scale = new(1, 1);
    [Tooltip("Multiplicateur simple appliqué à la scale totale, si besoin de contrôler juste la scale globale " +
             "au lieu des composants x et y individuels.")]
    [SerializeField] internal FloatCurve scaleMultiplier = 1f;
    [Tooltip("Désactiver le calcul (optimisation).")]
    [SerializeField] internal bool disableScale = false;

    public override BulletTransformInstance Instance() => new BTGeneralInstance(this);
}

public class BTGeneralInstance : BulletTransformInstance
{
    private readonly bool _disableForwardSpeed;
    private Vector2CurveInstance _forwardSpeed;
    private readonly bool _disableWorldSpeed;
    private Vector2CurveInstance _worldSpeed;
    private readonly bool _disablePFM;
    private FloatCurveInstance _playerFollowMultiplier;
    private readonly bool _disableRotationSpeed;
    private FloatCurveInstance _rotationSpeed;
    private readonly bool _disableScale;
    private Vector2CurveInstance _scale;
    private FloatCurveInstance _scaleMultiplier;

    public BTGeneralInstance(BTGeneral data) : base(data)
    {
        _disableForwardSpeed = data.disableForwardSpeed;
        _forwardSpeed = data.forwardSpeed.Instance();
        _disableWorldSpeed = data.disableWorldSpeed;
        _worldSpeed = data.worldSpeed.Instance();
        _disablePFM = data.disablePFM;
        _playerFollowMultiplier = data.playerFollowMultiplier.Instance();
        _disableRotationSpeed = data.disableRotationSpeed;
        _rotationSpeed = data.rotationSpeed.Instance();
        _disableScale = data.disableScale;
        _scale = data.scale.Instance();
        _scaleMultiplier = data.scaleMultiplier.Instance();
    }

    protected override (Vector2, float, Vector2) TransformTimed(Vector2 forward, float time)
    {
        Vector2 p = Vector2.zero; // Position
        float r = !_disableRotationSpeed ? _rotationSpeed.Eval(time) * Time.fixedDeltaTime : 0f; // Rotation
        Vector2 s = !_disableScale ? _scale.Eval(time) * _scaleMultiplier.Eval(time) : Vector2.one; // Scale

        if (!_disableForwardSpeed)
        {
            Vector2 right = new(forward.y, forward.x);
            Vector2 velocity = _forwardSpeed.Eval(time);
            p += forward * (velocity.x * Time.fixedDeltaTime) + right * (velocity.y * Time.fixedDeltaTime);
        }
        if (!_disableWorldSpeed)
            p += _worldSpeed.Eval(time) * Time.fixedDeltaTime;
        if (!_disablePFM) 
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