using System.Collections;
using UnityEngine;

[SelectionBase]
public class Bullet : MonoBehaviour, IBulletMovable
{
    private BulletMovement _m;
    private BulletProperties _p;
    private Pool<Bullet> _pool;
    private Coroutine _releaseRoutine;
    private float _startTime;
    private float _moveRandom;
    private float _powerRandom;
    
    private float Lifetime => Time.time - _startTime;
    public Vector2 Forward => transform.right;

    public void Init(Pool<Bullet> pool, BulletMovement movement, BulletProperties properties)
    {
        _pool = pool;
        _m = movement;
        _p = properties;
        _startTime = Time.time;
        _moveRandom = Random.value;
        _powerRandom = Random.value;
        
        _releaseRoutine = StartCoroutine(ReleaseCoroutine());
    }

    private IEnumerator ReleaseCoroutine()
    {
        yield return new WaitForSeconds(_p.GetLifetime(Random.value));
        
        _pool.Release(this);
    }

    private void FixedUpdate()
    {
        _m.Move(this, Lifetime, _moveRandom);
    }

    public void Move(Vector2 delta)
    {
        transform.Translate(delta * Time.fixedDeltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_p.impactType != BulletProperties.ImpactType.None && collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            switch (_p.impactType)
            {
                case BulletProperties.ImpactType.Damage:
                    playerHealth.TakeDamage((int)_p.GetImpactPower(Lifetime, _powerRandom));
                    break;
                case BulletProperties.ImpactType.Heal:
                    playerHealth.Heal((int)_p.GetImpactPower(Lifetime, _powerRandom));
                    break;
            }
            
            StopCoroutine(_releaseRoutine);
            _pool.Release(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)Forward * 0.2f);
    }
}
