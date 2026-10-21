using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FillManager : MonoBehaviour
{
    private static readonly int[] HitTriggers =
    {
        Animator.StringToHash("Level_1"),
        Animator.StringToHash("Level_2"),
        Animator.StringToHash("Level_3")
    };
    private const int NoSkill = -1;
    
    [Header("Parameters")]
    [SerializeField] private float[] skillFractions;
    [SerializeField] private int[] attackPowers;
    [SerializeField] private UnityEvent[] skillUnlockedEvents;
    [SerializeField] private UnityEvent attackEvent;

    [Header("References")]
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
}
