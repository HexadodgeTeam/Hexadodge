using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelSelection : MonoBehaviour
{
    private const int SlotLeft = 0;
    private const int SlotCenter = 1;
    private const int SlotRight = 2;
    private const int SlotNextFloor = 3;
    private const int SlotPrevFloor = 4;

    private const int LevelsPerFloor = 3;
    private const float FloorHeight = 11f;
    private const float MinSwipeDistance = 50f;

    [SerializeField] private List<RuntimeAnimatorController> enemyAnim = new();
    [SerializeField] private List<GameObject> enemyanimator = new();
    [SerializeField] private List<GameObject> enemy = new();
    [SerializeField] private List<FightData> fights = new();
    [SerializeField] private List<string> sceneName;
    [SerializeField] private GameObject BG;
    [SerializeField] private TextMeshProUGUI ennemyName;
    [SerializeField] private float changeSpeed;

    public int currentLevel = 0;
    public int previousLevel = 0;
    public int actualFloor = 0;
    public bool isOpen = false;

    private RectTransform[] rects;  
    private Animator[] anims;      
    private Vector2[] slots;        
    private Vector2 swipeStart;
    private bool isChanging;
    
    private const int AnimCenter = 0;
    private const int AnimRight  = 1;
    private const int AnimLeft   = 2;
    private static readonly int SlideLeftHash  = Animator.StringToHash("Slide_Left");
    private static readonly int SlideRightHash = Animator.StringToHash("Slide_Right");

    private bool CanChange => !isChanging && rects != null && enemyAnim.Count > 0;
    private int FloorCount => Mathf.CeilToInt(enemyAnim.Count / (float)LevelsPerFloor);
    private FightData CurrentFight => fights[currentLevel < fights.Count ? currentLevel : 0];
    private int FloorSize(int floor) => Mathf.Clamp(enemyAnim.Count - floor * LevelsPerFloor, 1, LevelsPerFloor);

    private static Vector3 BgPosition(int floor) => new Vector3(0f, -floor * FloorHeight, 0f);

    private void Awake()
    {
        if (enemy.Count < 5 || enemyanimator.Count < 5)
        {
            Debug.LogError("LevelSelection : 'enemy' et 'enemyanimator' doivent contenir 5 éléments.", this);
            enabled = false;
            return;
        }

        rects = enemy.ConvertAll(e => e.GetComponent<RectTransform>()).ToArray();
        anims = enemyanimator.ConvertAll(e => e.GetComponent<Animator>()).ToArray();
        slots = System.Array.ConvertAll(rects, r => r.anchoredPosition);

        isOpen = false;
        actualFloor = currentLevel / LevelsPerFloor;
        Refresh();
    }
public string GetScene()
    {
        return sceneName[currentLevel];
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

        int trigger = forward ? SlideLeftHash : SlideRightHash;
        anims[AnimCenter].SetTrigger(trigger);
        anims[forward ? AnimRight : AnimLeft].SetTrigger(trigger);

        int exit = forward ? SlotLeft : SlotRight;
        int entry = forward ? SlotRight : SlotLeft;
        StartCoroutine(Transition(exit, entry, entry));
    }
    public void ChangeFloorL(int dir)
    {
        int target = actualFloor + (dir > 0 ? 1 : -1);
        if (!CanChange || dir == 0 || target < 0 || target >= FloorCount)
            return;

        previousLevel = currentLevel;
        actualFloor = target;
        currentLevel = target * LevelsPerFloor;
        bool next = dir > 0;
        int exit = next ? SlotNextFloor : SlotPrevFloor;
        int from = next ? SlotPrevFloor : SlotNextFloor;
        StartCoroutine(Transition(exit, exit, from));
    }

    private IEnumerator Transition(int exitSlot, int entryIndex, int entryStartSlot)
    {
        isChanging = true;

        RectTransform center = rects[SlotCenter];
        RectTransform entry = rects[entryIndex];

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

            center.anchoredPosition = Vector2.Lerp(centerFrom, centerTo, t);
            entry.anchoredPosition = Vector2.Lerp(entryFrom, entryTo, t);
            BG.transform.position = Vector3.Lerp(bgFrom, bgTo, t); 

            yield return null;
        }

        Refresh();
        isChanging = false;
    }
    private void Refresh()
    {
        int first = actualFloor * LevelsPerFloor;
        int size = FloorSize(actualFloor);
        int col = currentLevel - first;

        SetController(anims[0], currentLevel);
        SetController(anims[1], first + (col + 1) % size);
        SetController(anims[2], first + (col + size - 1) % size);
        SetController(anims[3], first + LevelsPerFloor);
        SetController(anims[4], first - LevelsPerFloor);

        for (int i = 0; i < rects.Length; i++)
            rects[i].anchoredPosition = slots[i];

        ennemyName.text = CurrentFight.title;
    }

    private void SetController(Animator anim, int level)
    {
        if (level < 0 || level >= enemyAnim.Count || anim.runtimeAnimatorController == enemyAnim[level])
            return; 

        anim.runtimeAnimatorController = enemyAnim[level];

        if (anim.isActiveAndEnabled)
            anim.Update(0f); 
    }
	public void SetFight()
    {
        FightDataTransfer.Instance.SetData(fights[currentLevel < fights.Count ? currentLevel : 0]);
    }
}