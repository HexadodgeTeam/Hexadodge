using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;

    public GameData gameData;

    public List<SkillData> ObtainedSkills { get; } = new();
    public List<SkillData> EquippedSkills { get; } = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void LoadData()
    {
        SaveData saveData = SaveSystem.Load();

        foreach (int skillId in saveData.obtainedSkillIds)
        {
            ObtainedSkills.Add(gameData.GetSkillByID(skillId));
        }
        
        foreach (int skillId in saveData.equippedSkillIds)
        {
            EquippedSkills.Add(gameData.GetSkillByID(skillId));
        }

        if (saveData.freshSave)
        {
            // Give start skills if fresh save.
            foreach (SkillData skill in gameData.startSkills)
            {
                ObtainedSkills.Add(skill);
            }
            
            SaveData();
        }
    }

    public void SaveData()
    {
        SaveData saveData = new SaveData
        {
            freshSave = false
        };

        foreach (SkillData skill in ObtainedSkills)
        {
            saveData.obtainedSkillIds.Add(skill.ID);
        }
        
        foreach (SkillData skill in EquippedSkills)
        {
            saveData.equippedSkillIds.Add(skill.ID);
        }
        
        SaveSystem.Save(saveData);
    }

    public void ResetData()
    {
        SaveSystem.Reset();
        LoadData();
    }

    /// <summary>
    /// Get a save with everything unlocked.
    /// </summary>
    public void LoadFullSave()
    {
        SaveData saveData = new SaveData
        {
            freshSave = false
        };

        foreach (SkillData skill in gameData.gameSkills)
        {
            ObtainedSkills.Add(skill);
        }
        
        SaveSystem.Save(saveData);
        LoadData();
    }
}