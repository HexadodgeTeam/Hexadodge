using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance;

    public GameData gameData;

    public List<SkillData> obtainedSkills { get; } = new();
    public List<SkillData> equippedSkills { get; } = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
            obtainedSkills.Add(gameData.GetSkillByID(skillId));
        }
        
        foreach (int skillId in saveData.equippedSkillIds)
        {
            equippedSkills.Add(gameData.GetSkillByID(skillId));
        }

        if (saveData.freshSave)
        {
            // Give start skills if fresh save.
            foreach (SkillData skill in gameData.startSkills)
            {
                obtainedSkills.Add(skill);
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

        foreach (SkillData skill in obtainedSkills)
        {
            saveData.obtainedSkillIds.Add(skill.ID);
        }
        
        foreach (SkillData skill in equippedSkills)
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
            obtainedSkills.Add(skill);
        }
        
        SaveSystem.Save(saveData);
        LoadData();
    }
}