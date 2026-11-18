using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardView : MonoBehaviour
{
    [Serializable]
    public class ItemRow
    {
        public Image[] slots;
    }

    [Header("Données")]
    public string levelId = "1-1";
    public LevelLeaderboard leaderboard;

    [Header("Lignes (même taille dans chaque liste)")]
    public Image[] avatarImages;
    public TMP_Text[] pseudoTexts;
    public TMP_Text[] scoreTexts;
    public TMP_Text[] resultTexts;
    public ItemRow[] itemRows;

    [Header("Sprites des items (index = id de l'item)")]
    public Sprite[] itemSprites;

    void OnEnable() => Refresh();

    public async void Refresh()
    {
        if (leaderboard == null) return;

        await PlayerIdentity.Init();
        var rows = await leaderboard.FetchTop(levelId, pseudoTexts.Length);
        if (this == null) return;

        for (int i = 0; i < pseudoTexts.Length; i++)
        {
            if (i < rows.Count) Fill(i, rows[i]);
            else Clear(i);
        }
    }
    void Fill(int i, LeaderboardRow row)
    {
        int.TryParse(row.attempts, out int attempts);
        int.TryParse(row.successes, out int successes);
        int fails = Mathf.Max(0, attempts - successes);

        
        var avatar = PlayerIdentity.GetAvatarSprite(row.avatarId);
        avatarImages[i].sprite = avatar;
        avatarImages[i].enabled = avatar != null;

        
        pseudoTexts[i].text = $"{row.rank}. {row.pseudo}";
        scoreTexts[i].text  = row.TimeText;
        resultTexts[i].text = $"{fails} échecs / {successes} réussites";

        
        var slots = itemRows[i].slots;
        for (int s = 0; s < slots.Length; s++)
        {
            Sprite sprite = null;
            if (s < row.items.Length
                && int.TryParse(row.items[s], out int itemId)
                && itemId >= 0 && itemId < itemSprites.Length)
            {
                sprite = itemSprites[itemId];
            }

            slots[s].sprite = sprite;
            slots[s].enabled = sprite != null;
        }
    }

    void Clear(int i)
    {
        avatarImages[i].enabled = false;
        pseudoTexts[i].text = "";
        scoreTexts[i].text = "";
        resultTexts[i].text = "";
        foreach (var img in itemRows[i].slots) img.enabled = false;
    }
}