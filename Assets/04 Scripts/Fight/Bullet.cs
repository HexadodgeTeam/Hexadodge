using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
public class Bullet : MonoBehaviour, IBulletMovable
{
    private BulletData _data;
    private BulletPropertiesInstance _properties;
    private BulletMovementInstance _movement;
    private Pool<Bullet> _pool;
    private float _age;
    private float _lifetime;
    private float _t;
    private Coroutine _releaseRoutine;
    
    public Vector2 Forward => transform.right;

    public void Init(Pool<Bullet> pool, BulletData data)
    {
        if (_data == data)
        {
            // If same data was already present on pooled object, just re-roll random values.
            _properties.ReRoll();
            _movement.ReRoll();
        }
        else
        {
            // If no data or different, instance fields.
            _data = data;
            _properties = _data.properties.Instance();
            _movement = _data.movement.Instance();
            _pool = pool;
        }
        
        _age = 0f;
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
        float scale = _properties.scale.Eval(_t);
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void FixedUpdate()
    {
        _movement.Move(this, _t);
    }

    public void Move(Vector2 delta)
    {
        transform.Translate(delta * Time.fixedDeltaTime, Space.World);
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
            float angle = i * arc / (bulletCount - 1);
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
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)Forward * 0.2f);
    }
}