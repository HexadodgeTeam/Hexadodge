using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private List<Heart> hearts;
    [SerializeField] private Heart heartPrefab;

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
    }
}