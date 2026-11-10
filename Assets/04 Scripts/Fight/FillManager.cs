using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class FillManager : MonoBehaviour
{
    private const int NoSkill = -1;
    
    [Header("Parameters")]
    [SerializeField] private float[] skillFractions;
    [SerializeField] private UnityEvent[] skillUnlockedEvents;

    [Header("References")]
    [SerializeField] private FillGrid grid;
    
    private int _currentSkillIndex = NoSkill;
    private readonly List<SkillData> _skills = new();

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
        foreach (SkillData skill in _skills)
        {
            skill.OnLevelReached(CurrentSkillIndex);
        }
    }

    public void Init(SkillData[] baseSkills)
    {
        foreach (SkillData skill in baseSkills)
        {
            _skills.Add(skill);
        }
    }
    
    private void SetFillFraction(float fraction)
    {
        while (CurrentSkillIndex + 1 < skillFractions.Length && fraction >= skillFractions[CurrentSkillIndex + 1])
        {
            CurrentSkillIndex++;
            foreach (SkillData skill in _skills)
            {
                skill.OnLevelReached(CurrentSkillIndex);
            }
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
