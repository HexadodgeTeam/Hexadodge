using UnityEngine;
using UnityEngine.Events;

public class FillManager : MonoBehaviour
{
    private const int NoSkill = -1;
    
    [Header("Parameters")]
    [SerializeField] private float[] skillFractions;
    [SerializeField] private UnityEvent[] skillUnlockedEvents;

    [Header("References")]
    [SerializeField] private FillGrid grid;
    
    private int _currentSkillIndex = NoSkill;

    public bool HasSkill => _currentSkillIndex != NoSkill;
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
        UIManager.Instance.SetThresholdIndicators(skillFractions);
        grid.OnFillUpdated = SetFillFraction;
    }
    
    private void SetFillFraction(float fraction)
    {
        while (CurrentSkillIndex + 1 < skillFractions.Length && fraction >= skillFractions[CurrentSkillIndex + 1])
        {
            CurrentSkillIndex++;
        }

        UIManager.Instance.fillUI.SetFraction(fraction);
    }

    public int GetSkillIndex()
    {
        return CurrentSkillIndex;
    }

    public void OnAttack()
    {
        grid.Empty(); // Empty grid.
        CurrentSkillIndex = NoSkill; // Reset skill index.
    }
}
