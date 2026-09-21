using System;
using UnityEngine;

[SelectionBase]
public class Bullet : MonoBehaviour, IBulletMovable
{
    [SerializeField] private int damage = 1;
    [SerializeField] private BulletMovementData movement;
    [SerializeField] private float despawnTime = 5f;

    public Vector2 Forward => transform.right;

    private void OnEnable()
    {
        Destroy(gameObject, despawnTime);
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
