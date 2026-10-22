using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[SelectionBase]
public class Bullet : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private LayerMask triggerLayerMask;
    [SerializeField] private LayerMask wallLayerMask;
    
    private const float BaseScaleFactor = 0.25f;
    [SerializeField] private BulletData _data;
    private BulletTransformInstance _transform;
    private BulletPropertiesInstance _properties;
    private BulletAnimationInstance _animation;
    private Pool<Bullet> _pool;
    private float _age;
    private float _lifetime;
    private float _t;
    private Coroutine _releaseRoutine;
    
    // Bullet pattern variables.
    private (bool x, bool y) _mirror;

    public void Init(Pool<Bullet> pool, BulletData data, (bool x, bool y) mirror)
    {
        if (!FightTest.ModifyOnPlay && _data == data)
        {
            // If same data was already present on pooled object, just re-roll random values.
            _transform.ReRollTransform(transform.position);
            _properties.ReRoll();
            _animation.ReRoll();
        }
        else
        {
            // If no data or different, init bullet.
            _data = data;
            _properties = _data.properties.Instance();
            _transform = _data.transform?.Instance(transform.position);
            _animation = _data.animation.Instance();
            _pool = pool;
            spriteRenderer.sprite = _data.animation.sprite;
            boxCollider.isTrigger = !data.properties.collider.isWall;
            circleCollider.isTrigger = !data.properties.collider.isWall;
            boxCollider.size = data.properties.collider.rectangleSize;
            circleCollider.radius = data.properties.collider.circleRadius;
            boxCollider.includeLayers = data.properties.collider.isWall ? wallLayerMask : triggerLayerMask;
            circleCollider.includeLayers = data.properties.collider.isWall ? wallLayerMask : triggerLayerMask;
            boxCollider.excludeLayers = data.properties.collider.isWall ? triggerLayerMask : wallLayerMask;
            circleCollider.excludeLayers = data.properties.collider.isWall ? triggerLayerMask : wallLayerMask;
            boxCollider.enabled = data.properties.collider.shape == BulletProperties.Collider.Shape.Rectangle;
            circleCollider.enabled = data.properties.collider.shape == BulletProperties.Collider.Shape.Circle;
        }

        _mirror = mirror;
        _t = 0f;
        _age = 0f;
        UpdateTransform();
        UpdateAnimation();
        _lifetime = _properties.Lifetime;
        _releaseRoutine = StartCoroutine(ReleaseCoroutine());
    }

    private IEnumerator ReleaseCoroutine()
    {
        yield return new WaitUntil(HasExpired);

        Die();
    }

    private void Update()
    {
        _age += Game.DeltaTime;
        _t = _age / _lifetime;
        
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        UpdateTransform();
    }

    private void UpdateAnimation()
    {
        spriteRenderer.color = _animation.GetColor(_t);
    }

    private void UpdateTransform()
    {
        (Vector2 position, float rotation, Vector2 scale) = _transform?.Transform(_mirror, transform.right, _t)
                                                            ?? (Vector2.zero, 0f, Vector2.one);
        
        transform.Translate(position, Space.World);
        transform.Rotate(Vector3.forward, rotation);
        transform.localScale = new Vector3(scale.x * BaseScaleFactor, scale.y * BaseScaleFactor, 1f);
    }

    private void Die()
    {
        SpawnExplosionProjectiles();
        _pool.Release(this);
    }

    private void SpawnExplosionProjectiles()
    {
        int bulletCount = _properties.ExplosionBulletCount.Eval(_t);
        float arc = _properties.ExplosionArc.Eval(_t);
        float angleOffset = _properties.RotationOffset.Eval(_t) + _data.properties.explosion.direction switch
        {
            BulletProperties.ExplosionDirection.BulletDirection => transform.eulerAngles.z,
            BulletProperties.ExplosionDirection.Player => Vector2.SignedAngle(Vector2.right,
                Player.Pos - (Vector2)transform.position),
            BulletProperties.ExplosionDirection.Random => Random.Range(0f, 360f),
            _ => throw new ArgumentOutOfRangeException()
        };
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = bulletCount == 1 ? 0 : i * arc / (bulletCount - 1);
            float rotation = angle - arc / 2f + angleOffset;
            Bullet bullet = _pool.Spawn(transform.position, rotation);
            if (bullet) bullet.Init(_pool, _data.properties.explosion.bullets.Length > 0
                ? _data.properties.explosion.bullets.RandomElement()
                : _data, (false, false));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsTangible()) return;

        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        PlayerCollision(playerHealth);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsTangible()) return;
        
        PlayerHealth playerHealth = collision.otherCollider.GetComponent<PlayerHealth>();

        PlayerCollision(playerHealth);
    }

    private void PlayerCollision(PlayerHealth playerHealth)
    {
        if (!playerHealth.IsInvincible)
        {
            switch (_data.properties.impactType)
            {
                case BulletProperties.ImpactType.Damage:
                    playerHealth.TakeDamage(_properties.ImpactPower.Eval(_t));
                    break;
                case BulletProperties.ImpactType.Heal:
                    playerHealth.Heal(_properties.ImpactPower.Eval(_t));
                    break;
            }

            if (_data.properties.destroyOnImpact)
            {
                StopCoroutine(_releaseRoutine);
                Die();
            }
        }
    }

    private bool IsTangible()
    {
        return _age > _data.properties.intangibleStart &&
               _age < _lifetime - _data.properties.intangibleEnd &&
               _data.properties.impactType != BulletProperties.ImpactType.None;
    }
    
    private bool HasExpired() => _age > _lifetime;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.2f);
    }
}