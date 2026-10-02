using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
public class Bullet : MonoBehaviour, IBulletMovable
{
    private BulletData _data;
    private Pool<Bullet> _pool;
    private Coroutine _releaseRoutine;
    private float _startTime;
    private float _moveRandom;
    private float _powerRandom;

    private BulletMovement M => _data.movement;
    private BulletData.Properties P => _data.properties;
    private float Lifetime => Time.time - _startTime;
    public Vector2 Forward => transform.right;

    public void Init(Pool<Bullet> pool, BulletData data)
    {
        _pool = pool;
        _data = data;
        _startTime = Time.time;
        _moveRandom = Random.value;
        _powerRandom = Random.value;
        
        _releaseRoutine = StartCoroutine(ReleaseCoroutine());
    }

    private IEnumerator ReleaseCoroutine()
    {
        yield return new WaitForSeconds(P.lifetime);
        
        Explode();
    }

    private void FixedUpdate()
    {
        M.Move(this, Lifetime, _moveRandom);
    }

    public void Move(Vector2 delta)
    {
        transform.Translate(delta * Time.fixedDeltaTime, Space.World);
    }

    private void Explode()
    {
        SpawnExplosionProjectiles();
        _pool.Release(this);
    }

    private void SpawnExplosionProjectiles()
    {
        int bulletCount = P.explosion.bulletCount;
        float arc = P.explosion.arc;
        float angleOffset = P.explosion.direction switch
        {
            BulletData.Properties.ExplosionDirection.BulletDirection => transform.eulerAngles.z,
            BulletData.Properties.ExplosionDirection.Player => Vector2.Angle(transform.position, Player.Pos),
            BulletData.Properties.ExplosionDirection.Random => Random.Range(0f, 360f),
            _ => throw new ArgumentOutOfRangeException()
        };
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * arc / bulletCount;
            float rotation = angle - arc / 2f + angleOffset;
            Bullet bullet = _pool.Spawn(transform.position, rotation);
            bullet?.Init(_pool, P.explosion.bullets.Length > 0 ? P.explosion.bullets.RandomElement() : _data);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (P.impactType == BulletData.Properties.ImpactType.None) return;
        
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (!playerHealth.IsInvincible)
            {
                switch (P.impactType)
                {
                    case BulletData.Properties.ImpactType.Damage:
                        playerHealth.TakeDamage(P.impactPower.Eval(Lifetime, _powerRandom));
                        break;
                    case BulletData.Properties.ImpactType.Heal:
                        playerHealth.Heal(P.impactPower.Eval(Lifetime, _powerRandom));
                        break;
                }
            
                StopCoroutine(_releaseRoutine);
                Explode();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)Forward * 0.2f);
    }
}
