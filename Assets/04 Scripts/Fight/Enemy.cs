using UnityEngine;

public class Enemy : MonoBehaviour
{
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Death = Animator.StringToHash("Death");

    [SerializeField] private Animator enemyAnimator;
    
    private int _health;
    private float _healthRatio;
    private EnemyData _data;

    private int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, _data.maxHealth);
            HealthRatio = _health / (float)_data.maxHealth;

            if (_health <= 0)
            {
                Die();
            }
        }
    }

    public float HealthRatio
    {
        get => _healthRatio;
        private set
        {
            _healthRatio = value;
            UIManager.Instance.enemyHealthUI.SetFraction(_healthRatio);
        }
    }

    public void Init(EnemyData data)
    {
        _data = data;
        Health = data.maxHealth;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        enemyAnimator.SetTrigger(Hit);
    }

    private void Die()
    {
        UIManager.Instance.GameOver(true);
        enemyAnimator.SetTrigger(Death);
    }
}
