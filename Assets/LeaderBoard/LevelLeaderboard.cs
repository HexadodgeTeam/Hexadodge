using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TaloGameServices;

public class LeaderboardRow
{
    public int rank;
    public int avatarId;
    public string pseudo;
    public float time;
    public string attempts;
    public string successes;
    public string[] items;

    public string TimeText => System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss\.ff");
}

public class LevelLeaderboard : MonoBehaviour
{
    const int MaxItems = 5;
    const string EmptySlot = "-1";
    const string DefaultPseudo = "Joueur";

    
    static string BoardName(string levelId) => levelId;

    
    public static async void OnLevelStart(string levelId)
    {
        Increment($"attempts_{levelId}");
    }

    static void Increment(string key)
    {
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key, 0) + 1);
        PlayerPrefs.Save();
    }

    
    public static async Task<bool> SubmitRun(string levelId, float timeSeconds, IList<int> equippedItemIds)
    {
        await PlayerIdentity.Init();
        Increment($"successes_{levelId}");

        var result = await Talo.Leaderboards.AddEntry(
            BoardName(levelId),
            timeSeconds,
            ("attempts", PlayerPrefs.GetInt($"attempts_{levelId}", 0).ToString()),
            ("successes", PlayerPrefs.GetInt($"successes_{levelId}", 0).ToString()),
            ("equipment", EncodeItems(equippedItemIds)),
            ("avatar", PlayerIdentity.GetAvatarId().ToString()),
            ("pseudo", PlayerIdentity.GetPseudo())
        );

        return result.Success && result.Updated;
    }

    static string EncodeItems(IList<int> ids)
    {
        var slots = new string[MaxItems];
        for (int i = 0; i < MaxItems; i++)
            slots[i] = ids != null && i < ids.Count ? ids[i].ToString() : EmptySlot;
        return string.Join(",", slots);
    }


    public async Task<List<LeaderboardRow>> FetchTop(string levelId, int limit = 10)
    {
        var res = await Talo.Leaderboards.GetTopEntries(BoardName(levelId), limit);
        var rows = new List<LeaderboardRow>();

        foreach (var entry in res.topEntries)
        {
            rows.Add(new LeaderboardRow
            {
                rank      = entry.position + 1,
                avatarId  = ReadAvatarId(entry),
                pseudo    = ReadPseudo(entry),
                time      = entry.score,
                attempts  = entry.GetProp("attempts", "0"),
                successes = entry.GetProp("successes", "0"),
                items     = ParseItems(entry.GetProp("equipment", ""))
            });
        }

        return rows;
    }

    string[] ParseItems(string raw)
    {
        var parts = raw.Split(',');
        var items = new string[MaxItems];
        for (int i = 0; i < MaxItems; i++)
            items[i] = i < parts.Length && !string.IsNullOrWhiteSpace(parts[i]) ? parts[i].Trim() : EmptySlot;
        return items;
    }

    int ReadAvatarId(LeaderboardEntry entry)
    {
        string value = entry.GetProp("avatar", "0");
        var player = entry.playerAlias?.player;
        if (player != null) value = player.GetProp("avatar", value);
        int.TryParse(value, out int id);
        return id;
    }

    string ReadPseudo(LeaderboardEntry entry)
    {
        string value = entry.GetProp("pseudo", "");
        var player = entry.playerAlias?.player;
        if (player != null) value = player.GetProp("pseudo", value);
        return string.IsNullOrWhiteSpace(value) ? DefaultPseudo : value;
    }
}