using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int version = 1;
    public bool freshSave = true;
    public List<FightSaveData> fights = new();
    public List<int> obtainedSkillIds = new(); // All skills in inventory.
    public List<int> equippedSkillIds = new(); // All equipped skills.
    public List<int> upcomingSkillIds = new(); // All skills that have been unlocked but not opened.

    [Serializable]
    public class FightSaveData
    {
        public bool unlocked = false;
        public bool beaten = false;
    }
}

public static class SaveSystem
{
    private static string Path => System.IO.Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        string tmp = Path + ".tmp";
        string bak = Path + ".bak";

        File.WriteAllText(tmp, JsonUtility.ToJson(data));

        if (File.Exists(Path))
        {
            // Old file is backed up (bak). Transfer is done through tmp.
            File.Replace(tmp, Path, bak);
        }
        else
        {
            File.Move(tmp, Path);
        }
    }

    public static SaveData Load()
    {
        return File.Exists(Path)
            ? JsonUtility.FromJson<SaveData>(File.ReadAllText(Path))
            : new SaveData();
    }

    public static void Reset()
    {
        Save(new SaveData());
    }
}