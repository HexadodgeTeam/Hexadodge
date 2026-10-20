using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private static readonly int IFrames = Animator.StringToHash("IF");
    
    [SerializeField] private Animator playerAnimator;
    [Serializable]
    public class Data
    {
        public int maxHealth = 3;
        public float invincibilityCooldown = 1;
    }

    private Data _data;

    private int _health;
    private float _invincibilityTimer = 0f;

    public bool IsInvincible => _invincibilityTimer > 0f;
    private int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, _data.maxHealth);
            UIManager.Instance.healthUI?.SetCount(_health);
            
            if (_health <= 0)
            {
                Die();
            }
        }
    }

    public void Init(Data data)
    {
        _data = data;
        Health = _data.maxHealth;
    }

    private void Update()
    {
        if (_invincibilityTimer > 0f)
        {
            playerAnimator.SetBool(IFrames, true);
            _invincibilityTimer -= Game.DeltaTime;
        }
        else
        {
            playerAnimator.SetBool(IFrames, false);
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsInvincible) return;
        
        UIManager.Instance.OnHit();
        Health -= amount;
        
        _invincibilityTimer = _data.invincibilityCooldown;
    }

    public void Heal(int amount)
    {
        Health += amount;
    }

    private void Die()
    {
        UIManager.Instance.GameOver(false);
    }
}