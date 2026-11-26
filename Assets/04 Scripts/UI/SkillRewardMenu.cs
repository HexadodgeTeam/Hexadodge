using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillRewardMenu : MonoBehaviour
{
    [SerializeField] private string conditionPrefix = "Level ";
    
    [Header("UI Elements")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image skillIcon;
    [SerializeField] private TMP_Text conditionField;
    [SerializeField] private TMP_Text rarityField;
    [SerializeField] private TMP_Text descriptionField;
    [SerializeField] private GameObject descriptionPanel;
    
    private void LoadReward(SkillData skillData)
    {
        skillIcon.sprite = skillData.Icon;
        conditionField.text = $"{conditionPrefix}{skillData.level}";
        rarityField.text = skillData.rarity;
        descriptionField.text = skillData.Description;
    }
    
    public void Open()
    {
        gameObject.SetActive(true);
        equipButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        
        NextReward();
    }

    public void Reveal()
    {
        SetSkillVisibility(true);
        PlayerState.Instance.ObtainedSkills.Add(PlayerState.Instance.UpcomingSkills[0]);
        PlayerState.Instance.UpcomingSkills.RemoveAt(0);
        PlayerState.Instance.SaveData();

        if (PlayerState.Instance.UpcomingSkills.Count > 0)
        {
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            equipButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(true);
        }
    }

    public void NextReward()
    {
        SetSkillVisibility(false);
        LoadReward(PlayerState.Instance.UpcomingSkills[0]);
        
        nextButton.gameObject.SetActive(false);
    }

    private void SetSkillVisibility(bool visible)
    {
        skillIcon.gameObject.SetActive(visible);
        descriptionPanel.SetActive(visible);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
