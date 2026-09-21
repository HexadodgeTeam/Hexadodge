using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Bullet : MonoBehaviour, IBulletMovable
{
    [SerializeField] private int damage = 1;
    [SerializeField] private BulletMovementData movement;
    [SerializeField] private float despawnTime = 3f;

    private Pool<Bullet> _pool;
    
    public Vector2 Forward => transform.right;

    public void Init(Pool<Bullet> pool)
    {
        _pool = pool;
        Destroy(gameObject, despawnTime);
    }

    private IEnumerator ReleaseCoroutine()
    {
        yield return new WaitForSeconds(despawnTime);
        
        _pool.Release(this);
    }

    private void FixedUpdate()
    {
        movement.Move(this);
    }

    public void Move(Vector2 delta)
    {
        transform.Translate(delta * Time.fixedDeltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            playerHealth.TakeDamage(damage);
            
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)Forward * 0.2f);
    }
}
