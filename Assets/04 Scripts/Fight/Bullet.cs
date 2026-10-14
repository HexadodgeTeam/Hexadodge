using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[SelectionBase]
public class Bullet : MonoBehaviour
{
    public static float BulletTimeScale = 1f;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private const float BulletScaleFactor = 0.25f;
    private BulletData _data;
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
        if (!BulletTester.ModifyOnPlay && _data == data)
        {
            // If same data was already present on pooled object, just re-roll random values.
            _transform.ReRollTransform(transform.position);
            _properties.ReRoll();
            _animation.ReRoll();
        }
        else
        {
            // If no data or different, instance fields.
            _data = data;
            _properties = _data.properties.Instance();
            _transform = _data.transform?.Instance(transform.position);
            _animation = _data.animation.Instance();
            _pool = pool;
            spriteRenderer.sprite = _data.animation.sprite;
        }

        _mirror = mirror;
        _t = 0f;
        _age = 0f;
        UpdateTransform();
        UpdateAnimation();
        _lifetime = _properties.lifetime;
        _releaseRoutine = StartCoroutine(ReleaseCoroutine());
    }

    private IEnumerator ReleaseCoroutine()
    {
        yield return new WaitUntil(HasExpired);

        Die();
    }

    private void Update()
    {
        _age += BulletTimeScale * Time.deltaTime;
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
        transform.localScale = new Vector3(scale.x * BulletScaleFactor, scale.y * BulletScaleFactor, 1f);
    }

    private void Die()
    {
        SpawnExplosionProjectiles();
        _pool.Release(this);
    }

    private void SpawnExplosionProjectiles()
    {
        int bulletCount = _properties.explosionBulletCount.Eval(_t);
        float arc = _properties.explosionArc.Eval(_t);
        float angleOffset = _properties.rotationOffset.Eval(_t) + _data.properties.explosion.direction switch
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
        if (_age < _properties.tangibleCooldown) return;
        if (_data.properties.impactType == BulletProperties.ImpactType.None) return;

        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (!playerHealth.IsInvincible)
        {
            switch (_data.properties.impactType)
            {
                case BulletProperties.ImpactType.Damage:
                    playerHealth.TakeDamage(_properties.impactPower.Eval(_t));
                    break;
                case BulletProperties.ImpactType.Heal:
                    playerHealth.Heal(_properties.impactPower.Eval(_t));
                    break;
            }

            if (_data.properties.destroyOnImpact)
            {
                StopCoroutine(_releaseRoutine);
                Die();
            }
        }
    }
    
    private bool HasExpired() => _age > _lifetime;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.2f);
    }
}