using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Serializable]
    public struct Data
    {
        public int MaxHealth;
        public float InvincibiltyCooldown;
    }

    [SerializeField] private Data data;

    private int _health;
    private float _invincibilityTimer = 0f;

    private bool IsInvicible => _invincibilityTimer > 0f;
    private int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(0, data.MaxHealth, value);
            
            if (_health <= 0)
            {
                Die();
            }
        }
    }

    private void OnEnable()
    {
        _health = data.MaxHealth;
    }

    private void Update()
    {
        if (_invincibilityTimer > 0f)
        {
            _invincibilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsInvicible) return;

        _health -= amount;
        _invincibilityTimer = data.InvincibiltyCooldown;
    }

    private void Die()
    {

    }
}