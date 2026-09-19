using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CountUI healthUI;
    [SerializeField] private SliderUI fillUI;
    [SerializeField] private Button attackButton;
    
    public static CountUI HealthUI;
    public static SliderUI FillUI;
    private static Button AttackButton;

    public static void SetAttackButton(bool ready)
    {
        AttackButton.interactable = ready;
    }
    
    private void Awake()
    {
        HealthUI = healthUI;
        FillUI = fillUI;
        AttackButton = attackButton;
    }
}
