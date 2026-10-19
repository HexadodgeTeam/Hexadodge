using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private static readonly int IFrames = Animator.StringToHash("IF");
    
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
            UIManager.Instance.healthUI?.SetCount(_health);
            
            if (_health <= 0)
            {
                Die();
            }
        }
    }

    private void Start()
    {
        Health = data.MaxHealth;
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
        
        _invincibilityTimer = data.InvincibilityCooldown;
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