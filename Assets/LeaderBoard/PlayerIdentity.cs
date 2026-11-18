using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TaloGameServices;

public class PlayerIdentity : MonoBehaviour
{
    const string GuidKey   = "device_guid";
    const string PseudoKey = "pseudo";
    const string AvatarKey = "avatar";

    public static PlayerIdentity Instance { get; private set; }
    static Task initTask;

    [Header("Avatars (index = id de l'avatar)")]
    public Sprite[] avatarSprites;

    [Header("Menu de choix de la pdp")]
    public Button[] choiceButtons;
    public Image preview;

    void Awake()
    {
        Instance = this;
    }

    async void Start()
    {
        await Init();
        if (this == null) return;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int id = i;
            choiceButtons[i].onClick.AddListener(() => Choose(id));
        }
        RefreshPreview();
    }

    public static Task Init()
    {
        if (initTask == null || initTask.IsFaulted) initTask = DoInit();
        return initTask;
    }

    static async Task DoInit()
    {
        if (!PlayerPrefs.HasKey(GuidKey))
            PlayerPrefs.SetString(GuidKey, Guid.NewGuid().ToString());

        await Talo.Players.Identify("device", PlayerPrefs.GetString(GuidKey));
    }


    public static string GetPseudo() => Talo.CurrentPlayer.GetProp(PseudoKey, "Joueur");

    public static async Task<bool> SetPseudo(string pseudo)
    {
        var result = await Talo.CurrentPlayer.SetProp(PseudoKey, pseudo);
        return result.RejectedProps.Length == 0;
    }

    public static async Task SetAvatarId(int id)
    {
        await Talo.CurrentPlayer.SetProp(AvatarKey, id.ToString());
    }


    public static int GetAvatarId()
    {
        int.TryParse(Talo.CurrentPlayer.GetProp(AvatarKey, "0"), out int id);
        return id;
    }

    public static Sprite GetAvatarSprite(int id)
    {
        var sprites = Instance != null ? Instance.avatarSprites : null;
        return sprites != null && id >= 0 && id < sprites.Length ? sprites[id] : null;
    }

    async void Choose(int id)
    {
        await SetAvatarId(id);
        RefreshPreview();
    }

    void RefreshPreview()
    {
        if (preview == null) return;
        preview.sprite = GetAvatarSprite(GetAvatarId());
    }
}