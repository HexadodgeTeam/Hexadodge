using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CountUI healthUI;
    
    public static CountUI HealthUI;

    private void Awake()
    {
        HealthUI = healthUI;
    }
}
