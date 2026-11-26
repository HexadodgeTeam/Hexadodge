using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;

    public GameData gameData;

    public List<SkillData> ObtainedSkills { get; } = new();
    public List<SkillData> EquippedSkills { get; } = new();
    public List<SkillData> UpcomingSkills { get; } = new();
    public Dictionary<FightData, SaveData.FightSaveData> Fights { get; } = new();

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

        if (saveData.freshSave) // If fresh save, instantiate what must be instantiated.
        {
            // Give start skills.
            foreach (SkillData skill in gameData.startSkills)
            {
                ObtainedSkills.Add(skill);
            }
            
            // Instantiate all fights.
            if (saveData.fights.Count == 0)
            {
                for (int i = 0; i < gameData.gameFights.Length; i++)
                {
                    FightData fight = gameData.gameFights[i];
                    Fights.Add(fight, new SaveData.FightSaveData());

                    if (i == 0) Fights[fight].unlocked = true; // Unlock first fight by default.
                }
            }
            
            SaveData();
        }
        else // Else, get data from save.
        {
            foreach (int skillId in saveData.obtainedSkillIds) 
                ObtainedSkills.Add(gameData.GetSkillByID(skillId));
        
            foreach (int skillId in saveData.equippedSkillIds)
                EquippedSkills.Add(gameData.GetSkillByID(skillId));

            foreach (int skillId in saveData.upcomingSkillIds)
                UpcomingSkills.Add(gameData.GetSkillByID(skillId));
            
            for (int i = 0; i < saveData.fights.Count; i++)
                Fights.Add(gameData.GetFightByID(i), saveData.fights[i]);
        }
    }

    public void SaveData()
    {
        SaveData saveData = new SaveData
        {
            freshSave = false
        };

        foreach (SkillData skill in ObtainedSkills)
            saveData.obtainedSkillIds.Add(skill.ID);
        
        foreach (SkillData skill in EquippedSkills) 
            saveData.equippedSkillIds.Add(skill.ID);
        
        foreach (SkillData skill in UpcomingSkills)
            saveData.upcomingSkillIds.Add(skill.ID);
        
        foreach ((_, SaveData.FightSaveData fightSaveData) in Fights)
            saveData.fights.Add(fightSaveData);
        
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

        foreach (SkillData skill in gameData.gameSkills) // All skills unlocked !!
        {
            ObtainedSkills.Add(skill);
        }

        for (int i = 0; i < gameData.gameFights.Length; i++) // All fights unlocked !!
        {
            FightData fight = gameData.gameFights[i];
            Fights.Add(fight, saveData.fights[i]);
            Fights[fight].unlocked = true;
        }
        
        SaveSystem.Save(saveData);
        LoadData();
    }
}