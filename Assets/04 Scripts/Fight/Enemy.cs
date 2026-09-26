using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private Animator enemyAnimator;

    private int _health;

    private int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, maxHealth);
            UIManager.EnemyHealthUI.SetFraction(_health / (float)maxHealth);

            if (_health <= 0)
            {
                Die();
            }
        }
    }

    private void Start()
    {
        Health = maxHealth;
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
