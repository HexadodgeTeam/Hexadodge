using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractFight : MonoBehaviour
{
    private static readonly int[] HitTriggers =
    {
        Animator.StringToHash("Level_1"),
        Animator.StringToHash("Level_2"),
        Animator.StringToHash("Level_3")
    };
    
    [Header("References")]
    [SerializeField] protected BulletSpawner bulletSpawner;
    [SerializeField] protected Enemy enemy;
    [SerializeField] private SpriteRenderer attackFade;
    [SerializeField] private TMP_Text damageText;
    
    [Header("Attack")]
    [SerializeField] private float attackDelay;
    [SerializeField] private Animator attackFx;
    [SerializeField] private SpriteRenderer attackFxSprite;
    [SerializeField] private UnityEvent attackEvent;
    [SerializeField] private Color attackFadeColor;
    
    private FightParameters _parameters;
    private int _attackBonus = 0;

    public void Init(FightParameters parameters)
    {
        _parameters = parameters;
    }
    
    public abstract void SetFight(FightData fightData);

    public abstract void EndFight();

    public void SetAttackColor(Color color)
    {
        StartCoroutine(SetAttackColorCoroutine(color));
    }

    public void AddAttackBonus(int value)
    {
        _attackBonus += value;
    }

    private IEnumerator SetAttackColorCoroutine(Color color)
    {
        // Wait for attack vfx animation to end.
        yield return new WaitForSeconds(0.8f);
        
        attackFxSprite.color = color;
    }
    
    public bool TryAttack(int skillIndex)
    {
        attackFx.SetTrigger(HitTriggers[skillIndex]);
        attackEvent.Invoke();
        int attackPower = _parameters.attackPowers[skillIndex] + _attackBonus;
        damageText.text = attackPower.ToString();
        UIManager.Instance.OnAttack();
        FillGrid.Instance.Active = false;
        StartCoroutine(AttackCoroutine(skillIndex));
        StartCoroutine(AttackSlowdownCoroutine());

        return true;
    }

    private IEnumerator AttackCoroutine(int skillIndex)
    {
        yield return new WaitForSeconds(attackDelay);
        
        enemy.TakeDamage(_parameters.attackPowers[skillIndex] + _attackBonus);
        yield return null;
        if (Game.Instance.IsInFight) FillGrid.Instance.Active = true;
    }

    private IEnumerator AttackSlowdownCoroutine()
    {
        float t = 0f;

        while (t < _parameters.attackSlowdownInDuration)
        {
            t += Time.deltaTime;
            float t01 = t / _parameters.attackSlowdownInDuration;
            
            Game.TimeScale = Mathf.LerpUnclamped(1f, _parameters.attackSlowdownTimeScale,
                _parameters.attackSlowdownIn.Evaluate(t01));
            attackFade.color = Color.LerpUnclamped(Color.clear, attackFadeColor, t01);

            yield return null;
        }

        yield return new WaitUntil(PlayerRetookControl);

        t = 0f;

        while (t < _parameters.attackSlowdownOutDuration)
        {
            t += Time.deltaTime;
            float t01 = t / _parameters.attackSlowdownOutDuration;

            Game.TimeScale = Mathf.LerpUnclamped(_parameters.attackSlowdownTimeScale, 1f,
                _parameters.attackSlowdownOut.Evaluate(t01));
            attackFade.color = Color.LerpUnclamped(attackFadeColor, Color.clear, t01);

            yield return null;
        }

        Game.TimeScale = 1f;
        attackFade.color = Color.clear;
    }

    private static bool PlayerRetookControl()
    {
        return PlayerInputHandler.PlayerTookControl;
    }

    public abstract void SetTransfer();
}