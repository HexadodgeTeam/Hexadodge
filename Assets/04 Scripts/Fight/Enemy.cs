using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    [SerializeField] private Animator enemyAnimator;
    private int _health;

    private int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, data.maxHealth);
            UIManager.EnemyHealthUI.SetFraction(_health / (float)data.maxHealth);

            if (_health <= 0)
            {
                Die();
            }
        }
    }

    private void Start()
    {
        Health = data.maxHealth;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        enemyAnimator.SetTrigger("Hit");
    }

    private void Die()
    {
        UIManager.GameOver(true);
        enemyAnimator.SetTrigger("Death");
    }
}
