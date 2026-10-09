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
        _forwardSpeed = data.forwardSpeed.Instance();
        _worldSpeed = data.worldSpeed.Instance();
        _playerFollowMultiplier = data.playerFollowMultiplier.Instance();
        _rotationSpeed = data.rotationSpeed.Instance();
        _scale = data.scale.Instance();
        _scaleMultiplier = data.scaleMultiplier.Instance();
    }

    protected override (Vector2, float, Vector2) TransformTimed(Vector2 forward, float lifetime01)
    {
        Vector2 p = Vector2.zero; // Position
        float r = _rotationSpeed.Eval(lifetime01) * Time.fixedDeltaTime; // Rotation
        Vector2 s = _scale.Eval(lifetime01) * _scaleMultiplier.Eval(lifetime01); // Scale

        Vector2 right = new(forward.y, forward.x);
        Vector2 velocity = _forwardSpeed.Eval(lifetime01);
        p += forward * (velocity.x * Time.fixedDeltaTime) + right * (velocity.y * Time.fixedDeltaTime);
        p += _worldSpeed.Eval(lifetime01) * Time.fixedDeltaTime;
        p += Player.Delta * _playerFollowMultiplier.Eval(lifetime01);
        
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