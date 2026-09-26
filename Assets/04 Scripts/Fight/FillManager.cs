using UnityEngine;
using UnityEngine.InputSystem;

public class FillManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float[] skillFractions;
    [SerializeField] private int[] attackPowers;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private FillGrid grid;
    
    private int _skillNumber;
    
    private void Start()
    {
        UIManager.SetThresholdIndicators(skillFractions);
        grid.OnFillUpdated = SetFillFraction;
    }
    
    private void SetFillFraction(float fraction)
    {
        _skillNumber = 0;

        foreach (float skillFraction in skillFractions)
        {
            if (fraction >= skillFraction)
            {
                _skillNumber++;
            }
        }

        UIManager.FillUI.SetFraction(fraction);
        UIManager.SkillNumber.SetCount(_skillNumber, true);
        UIManager.SetAttackButton(_skillNumber > 0);
    }

    public void TryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_skillNumber <= 0) return;
        
            enemy.TakeDamage(attackPowers[_skillNumber - 1]);
            UIManager.SetAttackButton(false);
            grid.EmptyAt(player.GridPos);
        }
    }
}
