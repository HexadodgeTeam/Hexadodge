using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    
    [SerializeField] private Player player;

    private void Awake()
    {
        Instance = this;
    }

    public void MultiplyFillRange(float multiplier)
    {
        player.FillRangeMultiplier *= multiplier;
    }
}