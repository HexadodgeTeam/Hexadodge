using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FillManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float[] skillFractions;
    [SerializeField] private int[] attackPowers;
    [SerializeField] private UnityEvent[] skillUnlockedEvents;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private FillGrid grid;
    
    private int _currentSkillIndex = -1;

    private int CurrentSkillIndex
    {
        get => _currentSkillIndex;
        set
        {
            _currentSkillIndex = value;
            if (_currentSkillIndex < skillUnlockedEvents.Length)
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
        if (CurrentSkillIndex + 1 < skillFractions.Length && fraction >= skillFractions[CurrentSkillIndex + 1])
        {
            CurrentSkillIndex++;
        }

        UIManager.FillUI.SetFraction(fraction);
        UIManager.SkillNumber.SetCount(CurrentSkillIndex + 1, true);
        UIManager.SetAttackButton(CurrentSkillIndex >= 0);
    }

    public void TryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CurrentSkillIndex < 0) return;
        
            enemy.TakeDamage(attackPowers[CurrentSkillIndex]);
            UIManager.SetAttackButton(false);
            grid.EmptyAt(player.GridPos);
        }
    }
}
