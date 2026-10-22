using System.Collections;
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
    
    [Header("Attack")]
    [SerializeField] private float attackDelay;
    [SerializeField] private Animator attackFx;
    [SerializeField] private UnityEvent attackEvent;
    
    private GameParameters _parameters;

    public void Init(GameParameters parameters)
    {
        _parameters = parameters;
    }
    
    public abstract void SetFight(FightData fightData);

    public abstract void EndFight();
    
    public bool TryAttack(int skillIndex)
    {
        attackFx.SetTrigger(HitTriggers[skillIndex]);
        attackEvent.Invoke();
        StartCoroutine(AttackCoroutine(skillIndex));
        StartCoroutine(AttackSlowdownCoroutine());

        return true;
    }

    private IEnumerator AttackCoroutine(int skillIndex)
    {
        yield return new WaitForSeconds(attackDelay);
        
        enemy.TakeDamage(_parameters.attackPowers[skillIndex]);
        yield return null;
    }

    private IEnumerator AttackSlowdownCoroutine()
    {
        float t = 0f;

        while (t < _parameters.attackSlowdownInDuration)
        {
            t += Time.deltaTime;

            Game.TimeScale = Mathf.LerpUnclamped(1f, _parameters.attackSlowdownTimeScale,
                _parameters.attackSlowdownIn.Evaluate(t / _parameters.attackSlowdownInDuration));

            yield return null;
        }

        yield return new WaitUntil(PlayerRetookControl);

        t = 0f;

        while (t < _parameters.attackSlowdownOutDuration)
        {
            t += Time.deltaTime;

            Game.TimeScale = Mathf.LerpUnclamped(_parameters.attackSlowdownTimeScale, 1f,
                _parameters.attackSlowdownOut.Evaluate(t / _parameters.attackSlowdownOutDuration));

            yield return null;
        }

        Game.TimeScale = 1f;
    }

    private static bool PlayerRetookControl()
    {
        return PlayerInputHandler.PlayerTookControl;
    }
}