using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private List<Heart> hearts;
    [SerializeField] private Heart heartPrefab;
    [SerializeField] private UnityEvent onLowHealth;

    private int _lastCount;

    private void Init(int maxHealth)
    {
        for (int i = 0; i < maxHealth; i++)
        {
            hearts.Add(Instantiate(heartPrefab, transform));
        }
    }
    
    public void Refresh(int count, Color color)
    {
        if (hearts.Count == 0) Init(count);
        
        for (int i = 0; i < count; i++)
        {
            hearts[i].Full(color);
        }

        for (int i = count; i < hearts.Count; i++)
        {
            hearts[i].Empty();
        }

        if (count != _lastCount && count == 1)
        {
            onLowHealth.Invoke();
        }

        _lastCount = count;
    }
}