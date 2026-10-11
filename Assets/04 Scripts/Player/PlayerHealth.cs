using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [Serializable]
    public struct Data
    {
        public int MaxHealth;
        public float InvincibilityCooldown;
    }

    [SerializeField] private Data data;

    private int _health;
    private float _invincibilityTimer = 0f;

    public bool IsInvincible => _invincibilityTimer > 0f;
    private int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, data.MaxHealth);
            UIManager.HealthUI?.SetCount(_health);
            
            if (_health <= 0)
            {
                Die();
            }
        }
    }

    private void OnEnable()
    {
        Health = data.MaxHealth;
    }

    private void Update()
    {
        if (_invincibilityTimer > 0f)
        {
            playerAnimator.SetBool("IF", true);
            _invincibilityTimer -= Time.deltaTime;
        }
        else
        {
            playerAnimator.SetBool("IF", false);
        }
    }

    public void TakeDamage(int amount)
    {
        
        if (IsInvincible) return;

        Health -= amount;
        
        _invincibilityTimer = data.InvincibilityCooldown;
    }

    public void Heal(int amount)
    {
        Health += amount;
    }

    private void Die()
    {
        UIManager.GameOver(false);
    }
}