using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[SelectionBase]
public class Bullet : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private const float BulletScaleFactor = 0.25f;
    private BulletData _data;
    private BulletPropertiesInstance _properties;
    private BulletTransformInstance _transform;
    private Pool<Bullet> _pool;
    private float _age;
    private float _lifetime;
    private float _t;
    private Coroutine _releaseRoutine;

    public void Init(Pool<Bullet> pool, BulletData data)
    {
        if (_data == data)
        {
            // If same data was already present on pooled object, just re-roll random values.
            _properties.ReRoll();
            _transform.ReRoll();
        }
        else
        {
            // If no data or different, instance fields.
            _data = data;
            _properties = _data.properties.Instance();
            _transform = _data.transform.Instance();
            _pool = pool;
            if (_data.animation.sprite) spriteRenderer.sprite = _data.animation.sprite;
        }

        _t = 0f;
        _age = 0f;
        UpdateTransform(0f, 0f);
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
        _age += Time.deltaTime;
        _t = _age / _lifetime;
    }

    private void FixedUpdate()
    {
        UpdateTransform(_t, _age);
    }

    private void UpdateTransform(float time, float realTime)
    {
        (Vector2 position, float rotation, Vector2 scale) = _transform.Transform(transform.right, time, realTime);
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
        float angleOffset = _data.properties.explosion.direction switch
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
                : _data);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_data.properties.impactType == BulletProperties.ImpactType.None) return;

        if (collision.CompareTag("Player"))
        {
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