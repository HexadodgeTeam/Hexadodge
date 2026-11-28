using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    private const int SlotCenter = 0;
    private const int SlotRight = 1;
    private const int SlotLeft = 2;
    private const int SlotNextFloor = 3;
    private const int SlotPrevFloor = 4;
    private const int SlotCount = 5;

    private const int LevelsPerFloor = 3;
    private const float FloorHeight = 11f;
    private const float MinSwipeDistance = 50f;

    [SerializeField] private List<RuntimeAnimatorController> enemyAnim = new();
    [SerializeField] private List<GameObject> enemyanimator = new();
    [SerializeField] private List<GameObject> enemy = new();
    [SerializeField] private List<Image> materials = new();
    [SerializeField] private List<Material> materialsToApply = new();
    [SerializeField] private List<FightData> fights = new();
    [SerializeField] private List<LeaderboardView> LeaderBoard = new();
    [SerializeField] private GameObject BG;
    [SerializeField] private TextMeshProUGUI ennemyName;
    [SerializeField] private float changeSpeed;
    [SerializeField] private Button playButton;

    public int currentLevel = 0;
    public int previousLevel = 0;
    public int actualFloor = 0;
    public bool isOpen = false;
    public Material[] runtimeMats;

    private RectTransform[] rects;
    private Animator[] anims;
    private Vector2[] slots;
    private Vector2 swipeStart;
    private bool isChanging;
    private int[] displayedLevels;

    private static readonly int SlideLeftHash = Animator.StringToHash("Slide_Left");
    private static readonly int SlideRightHash = Animator.StringToHash("Slide_Right");
    private static readonly int Unlocked = Animator.StringToHash("Unlocked");
    private static readonly int Beaten = Animator.StringToHash("Beaten");
    private static readonly int IsLocked = Shader.PropertyToID("_isLocked");
    private static readonly int IsComplete = Shader.PropertyToID("_isComplete");

    private bool CanChange => !isChanging && rects != null && enemyAnim.Count > 0;
    private int FloorCount => Mathf.CeilToInt(enemyAnim.Count / (float)LevelsPerFloor);
    private FightData CurrentFight => fights[currentLevel < fights.Count ? currentLevel : 0];
    private int FloorSize(int floor) => Mathf.Clamp(enemyAnim.Count - floor * LevelsPerFloor, 1, LevelsPerFloor);

    private static Vector3 BgPosition(int floor) => new Vector3(0f, -floor * FloorHeight, 0f);

    private void Awake()
    {
        if (enemy.Count < SlotCount || enemyanimator.Count < SlotCount || materials.Count < SlotCount)
        {
            Debug.LogError("LevelSelection : 'enemy', 'enemyanimator' et 'materials' doivent contenir 5 éléments " +
                           "(Center, Right, Left, Next floor, Prev floor).", this);
            enabled = false;
            return;
        }

        rects = enemy.ConvertAll(e => e.GetComponent<RectTransform>()).ToArray();
        anims = enemyanimator.ConvertAll(e => e.GetComponent<Animator>()).ToArray();
        slots = Array.ConvertAll(rects, r => r.anchoredPosition);

        isOpen = false;
        actualFloor = currentLevel / LevelsPerFloor;
        runtimeMats = materialsToApply.ConvertAll(m => new Material(m)).ToArray();
    }

    private void Start()
    {
        UpdateMaterials();
        Refresh();
        SoundManager.Instance.PlaySound("Ambiance");
    }

    private void OnDestroy()
    {
        if (runtimeMats == null) return;
        foreach (var m in runtimeMats)
            if (m != null) Destroy(m);
    }

    private void Update()
    {
        if (isOpen || Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
            swipeStart = touch.position.ReadValue();

        if (!touch.press.wasReleasedThisFrame)
            return;

        Vector2 delta = touch.position.ReadValue() - swipeStart;
        if (delta.magnitude < MinSwipeDistance)
            return;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            ChangeLevel(delta.x > 0 ? -1 : 1);
        else
            ChangeFloorL(delta.y > 0 ? -1 : 1);
    }

    public void ChangeSpecificLevel(int lvl)
    {
        if (!CanChange || lvl < 0 || lvl >= enemyAnim.Count)
            return;

        previousLevel = currentLevel;
        currentLevel = lvl;
        actualFloor = lvl / LevelsPerFloor;

        BG.transform.position = BgPosition(actualFloor);
        ShowLeaderboard(currentLevel % LevelsPerFloor);
        Refresh();
    }

    public void ChangeLevel(int dir)
    {
        if (!CanChange || dir == 0)
            return;

        int size = FloorSize(actualFloor);
        if (size < 2)
            return;

        bool forward = dir > 0;
        int first = actualFloor * LevelsPerFloor;

        previousLevel = currentLevel;
        currentLevel = first + (currentLevel - first + (forward ? 1 : size - 1)) % size;

        ShowLeaderboard(currentLevel % LevelsPerFloor);

        int trigger = forward ? SlideLeftHash : SlideRightHash;
        anims[SlotCenter].SetTrigger(trigger);
        anims[forward ? SlotRight : SlotLeft].SetTrigger(trigger);

        int exit = forward ? SlotLeft : SlotRight;
        int entry = forward ? SlotRight : SlotLeft;
        SoundManager.Instance.PlaySound("Swipe");
        StartCoroutine(Transition(exit, entry, entry));
    }

    public void ChangeFloorL(int dir)
    {
        int target = actualFloor + (dir > 0 ? 1 : -1);
        if (!CanChange || dir == 0 || target < 0 || target >= FloorCount)
            return;

        ShowLeaderboard(0, false);

        previousLevel = currentLevel;
        actualFloor = target;
        currentLevel = target * LevelsPerFloor;

        bool next = dir > 0;
        int exit = next ? SlotNextFloor : SlotPrevFloor;
        int from = next ? SlotPrevFloor : SlotNextFloor;
        SoundManager.Instance.PlaySound("Swipe");
        StartCoroutine(Transition(exit, exit, from));
    }

    private IEnumerator Transition(int exitSlot, int entryIndex, int entryStartSlot)
    {
        isChanging = true;
        SetMaterial(materials[0], previousLevel);

        RectTransform centerRect = rects[SlotCenter];
        RectTransform entryRect = rects[entryIndex];

        Vector2 centerFrom = slots[SlotCenter];
        Vector2 centerTo = slots[exitSlot];
        Vector2 entryFrom = slots[entryStartSlot];
        Vector2 entryTo = slots[SlotCenter];

        Vector3 bgFrom = BG.transform.position;
        Vector3 bgTo = BgPosition(actualFloor);

        float duration = Mathf.Max(changeSpeed, 0.01f);

        for (float t = 0f; t < 1f;)
        {
            t = Mathf.Min(1f, t + Time.deltaTime / duration);

            centerRect.anchoredPosition = Vector2.Lerp(centerFrom, centerTo, t);
            entryRect.anchoredPosition = Vector2.Lerp(entryFrom, entryTo, t);
            BG.transform.position = Vector3.Lerp(bgFrom, bgTo, t);

            yield return null;
        }

        Refresh();
        isChanging = false;
    }

    private void Refresh()
    {
        for (int i = 0; i < SlotCount; i++)
            rects[i].anchoredPosition = slots[i];

        RefreshMatAnim();

        ennemyName.text = CurrentFight.title;
        playButton.interactable = IsUnlocked(currentLevel);
    }
    private int[] SlotLevels()
    {
        int first = actualFloor * LevelsPerFloor;
        int size = FloorSize(actualFloor);
        int col = currentLevel - first;

        int[] levels = new int[SlotCount];
        levels[SlotCenter] = currentLevel;
        levels[SlotRight] = first + (col + 1) % size;
        levels[SlotLeft] = first + (col + size - 1) % size;
        levels[SlotNextFloor] = first + LevelsPerFloor;
        levels[SlotPrevFloor] = first - LevelsPerFloor;
        return levels;
    }

    private void RefreshMatAnim()
    {
        UpdateMaterials();

        int[] levels = SlotLevels();
        for (int i = 0; i < SlotCount; i++)
        {
            SetController(anims[i], levels[i]);
            SetMaterial(materials[i], levels[i]);
        }

        anims[SlotCenter].SetBool(Unlocked, IsUnlocked(currentLevel));
        anims[SlotCenter].SetBool(Beaten, IsBeaten(currentLevel));
    }
    private void UpdateMaterials()
    {
        int count = Mathf.Min(runtimeMats.Length, fights.Count);
        for (int i = 0; i < count; i++)
        {
            if (PlayerState.Instance.Fights.TryGetValue(fights[i], out var data))
            {
                runtimeMats[i].SetFloat(IsLocked, data.unlocked ? 0f : 1f);
                runtimeMats[i].SetFloat(IsComplete, data.beaten ? 1f : 0f);
                int[] levels = SlotLevels();
                displayedLevels = levels;
            }
            
        }
    }
    private void LateUpdate()
    {
        if (displayedLevels == null) return;

        for (int i = 0; i < SlotCount; i++)
        {
            int level = displayedLevels[i];
            if (level < 0 || level >= runtimeMats.Length) continue;

            if (materials[i].material != runtimeMats[level])
                materials[i].material = runtimeMats[level];
        }
    }

    private void SetMaterial(Image img, int level)
    {
        if (level < 0 || level >= runtimeMats.Length)
            return;

        img.material = runtimeMats[level];
    }
    private void SetController(Animator anim, int level)
    {
        if (level < 0 || level >= enemyAnim.Count || anim.runtimeAnimatorController == enemyAnim[level])
            return;

        anim.runtimeAnimatorController = enemyAnim[level];

        if (anim.isActiveAndEnabled)
        {
            anim.SetBool(Unlocked, IsUnlocked(level));
            anim.SetBool(Beaten, IsBeaten(level));
            anim.Update(0f);
        }
    }
    private void ShowLeaderboard(int index, bool refresh = true)
    {
        if (LeaderBoard.Count < LevelsPerFloor * 2)
            return;

        for (int i = 0; i < LevelsPerFloor; i++)
        {
            bool active = i == index;
            LeaderBoard[i].gameObject.SetActive(active);
            LeaderBoard[i + LevelsPerFloor].gameObject.SetActive(active);

            if (active && refresh)
                LeaderBoard[i].Refresh();
        }
    }

    private bool IsUnlocked(int level)
    {
        return level >= 0 && level < fights.Count &&
               PlayerState.Instance.Fights.TryGetValue(fights[level], out var d) &&
               d.unlocked;
    }

    private bool IsBeaten(int level)
    {
        return level >= 0 && level < fights.Count &&
               PlayerState.Instance.Fights.TryGetValue(fights[level], out var d) &&
               d.beaten;
    }

    public void SetFight()
    {
        FightDataTransfer.Instance.SetData(CurrentFight);
    }
}
