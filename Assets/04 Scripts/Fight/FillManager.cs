using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FillManager : MonoBehaviour
{
    private const int NoSkill = -1;
    
    [Header("Parameters")]
    [SerializeField] private float[] skillFractions;
    [SerializeField] private int[] attackPowers;
    [SerializeField] private UnityEvent[] skillUnlockedEvents;
    [SerializeField] private UnityEvent attackEvent;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private FillGrid grid;
    
    private int _currentSkillIndex = NoSkill;

    private bool HasSkill => _currentSkillIndex != NoSkill;
    private int CurrentSkillIndex
    {
        get => _currentSkillIndex;
        set
        {
            _currentSkillIndex = value;
            if (HasSkill && _currentSkillIndex < skillUnlockedEvents.Length)
                skillUnlockedEvents[_currentSkillIndex].Invoke();
        }
    }
    
    private void Start()
    {
        UIManager.SetThresholdIndicators(skillFractions);
        grid.OnFillUpdated = SetFillFraction;
    }
    
    private void SetFillFraction(float fraction)
    {
        while (CurrentSkillIndex + 1 < skillFractions.Length && fraction >= skillFractions[CurrentSkillIndex + 1])
        {
            CurrentSkillIndex++;
        }

        UIManager.FillUI.SetFraction(fraction);
    }

    public void TryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!HasSkill) return;
        
            enemy.TakeDamage(attackPowers[CurrentSkillIndex]);
            grid.EmptyAt(player.GridPos);
            CurrentSkillIndex = NoSkill;
            attackEvent.Invoke();
        }
    }
}
