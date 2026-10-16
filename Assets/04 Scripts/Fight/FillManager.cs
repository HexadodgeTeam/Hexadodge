using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    [SerializeField] private float enemyHitDelay = 0.5f;
    [SerializeField] private AnimationCurve attackSlowdownTimeScale;
    [SerializeField] private float attackSlowdownDuration;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private FillGrid grid;
    [SerializeField] private Animator attackFx;
    
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

    public void TryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!HasSkill) return;
            
            attackFx.SetTrigger(HitTriggers[CurrentSkillIndex]);
            grid.Empty();
            attackEvent.Invoke();
            StartCoroutine(AttackCoroutine(CurrentSkillIndex));
            StartCoroutine(AttackSlowdownCoroutine());
            CurrentSkillIndex = NoSkill;
        }
    }

    private IEnumerator AttackCoroutine(int skillIndex)
    {
        yield return new WaitForSeconds(enemyHitDelay);
        
        enemy.TakeDamage(attackPowers[skillIndex]);
    }

    private IEnumerator AttackSlowdownCoroutine()
    {
        float t = 0f;

        while (t < attackSlowdownDuration)
        {
            t += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Max(0f, attackSlowdownTimeScale.Evaluate(t / attackSlowdownDuration));

            yield return null;
        }

        Time.timeScale = 1f;
    }
}
