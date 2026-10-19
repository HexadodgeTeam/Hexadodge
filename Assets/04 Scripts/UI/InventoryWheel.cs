using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWheel : MonoBehaviour
{
    private const float StepAngle = 42f;
    private const float HalfStepAngle = StepAngle / 2f;
    private const int InventoryImageOffset = 5;

    [Header("Données")]
    public List<ItemData> ItemInInventory = new List<ItemData>();
    public List<ItemData> ItemEquip = new List<ItemData>();
    public List<Image> itemEmplacement = new List<Image>();
    public Sprite emptySlotSprite; 

    [Header("Focus et description")]
    public bool IsInventory;
    public TextMeshProUGUI nameItem;
    public TextMeshProUGUI descriptionItem;
    public TextMeshProUGUI lore;
    public int itemToShowInventaire;    
    public int itemToShowEquipement;   
    public Vector3 Deplacementpop;
    public Vector3 DeplacementInitial;

    [Header("Roue inventaire")]
    public Scrollbar scrollbarInventaire;
    public List<RectTransform> PivotSlotInventaire = new List<RectTransform>();
    public List<RectTransform> SlotInventaire = new List<RectTransform>();
    public List<float> RotationInitialSlotInventaire = new List<float>();
    public List<bool> isSlotInventaireShow = new List<bool>();
    public List<int> indexItemInventaire = new List<int>();
    public float vitesseScroll = -10;
    public Vector3 popScaleSlotInventaire;
    public Vector3 positionBaseSlotInventaire;
    public GameObject parentPop;
    public float popRange;

    [Header("Roue équipement")]
    public Scrollbar scrollbarEquipement;
    public List<RectTransform> PivotSlotEquipement = new List<RectTransform>();
    public List<RectTransform> SlotEquipement = new List<RectTransform>();
    public List<float> RotationInitialItemEquipement = new List<float>();
    public List<bool> isSlotEquipShow = new List<bool>();
    public float vitesseScrollEquip = -10;
    public Vector3 popScaleEquip;
    public float popRangeEquip;

    [Header("Animation de swap")]
    public float moveDuration = 0.15f;   

    private int EquipSlotCount => Mathf.Min(PivotSlotEquipement.Count, InventoryImageOffset);

    private int InventoryIndexStep => PivotSlotInventaire.Count;

    private readonly List<Quaternion> slotBaseLocalRotation = new List<Quaternion>();

    private bool isSwapping;


    public void changeDesciptionFocus(bool isInventoryB)
    {
        IsInventory = isInventoryB;
        RefreshDescription();
    }

    public void SwapItem()
    {
        if (isSwapping) return;
        StartCoroutine(SwapItemCoroutine());
    }
    private IEnumerator SwapItemCoroutine()
    {
        int inventorySlot = isSlotInventaireShow.IndexOf(true);
        int equipSlot = itemToShowEquipement;
        ItemData fromInventory = GetInventoryItem(itemToShowInventaire);

        if (inventorySlot < 0 || fromInventory == null
            || !IsValidEquipSlot(equipSlot)
            || equipSlot >= SlotEquipement.Count)
            yield break;

        isSwapping = true;

        RectTransform inventoryRect = SlotInventaire[inventorySlot];
        RectTransform equipRect = SlotEquipement[equipSlot];
        Vector3 inventoryLocalPos = inventoryRect.localPosition;
        Vector3 inventoryScale = inventoryRect.localScale;
        Vector3 equipLocalPos = equipRect.localPosition;
        Vector3 equipScale = equipRect.localScale;
        Vector3 inventoryWorldPos = inventoryRect.position;
        Vector3 equipWorldPos = equipRect.position;

        try
        {
            yield return StartCoroutine(Animate(moveDuration, t =>
            {
                inventoryRect.position = Vector3.Lerp(inventoryWorldPos, equipWorldPos, t);
                equipRect.position = Vector3.Lerp(equipWorldPos, inventoryWorldPos, t);

                inventoryRect.localScale = Vector3.Lerp(inventoryScale, equipScale, t);
                equipRect.localScale = Vector3.Lerp(equipScale, inventoryScale, t);
            }));
            ApplySwap(fromInventory, equipSlot);
        }
        finally
        {
            if (inventoryRect != null)
            {
                inventoryRect.localPosition = inventoryLocalPos;
                inventoryRect.localScale = inventoryScale;
            }

            if (equipRect != null)
            {
                equipRect.localPosition = equipLocalPos;
                equipRect.localScale = equipScale;
            }

            isSwapping = false;
        }
    }

    private void ApplySwap(ItemData fromInventory, int equipSlot)
    {
        int inventoryIndex = ItemInInventory.IndexOf(fromInventory);
        if (inventoryIndex < 0 || !IsValidEquipSlot(equipSlot)) return;

        ItemData fromEquipment = ItemEquip[equipSlot];

        ItemInInventory.RemoveAt(inventoryIndex);
        ItemEquip[equipSlot] = fromInventory;

        if (fromEquipment != null)
            ItemInInventory.Insert(inventoryIndex, fromEquipment);  

        OnItemsChanged();
    }

    private static IEnumerator Animate(float duration, System.Action<float> apply)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            apply(Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        apply(1f);
    }

    public void RemoveItem()
    {
        if (isSwapping) return;
        if (!IsValidEquipSlot(itemToShowEquipement)) return;

        ItemData item = ItemEquip[itemToShowEquipement];
        if (item == null) return;

        InsertInInventoryById(item);
        ItemEquip[itemToShowEquipement] = null;

        OnItemsChanged();
    }

    private void InsertInInventoryById(ItemData item)
    {
        int insertAt = Mathf.Clamp(item.ID, 0, ItemInInventory.Count);
        ItemInInventory.Insert(insertAt, item);
    }

    private void Start()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        PadEquipList();
        CacheSlotBaseRotations();
        InitInventoryIndexes();
        OnItemsChanged();
    }

    private void Update()
    {
        if (isSwapping) return;   

        UpdateInventoryWheel();
        UpdateEquipmentWheel();
    }

    private bool ValidateSetup()
    {
        int invCount = PivotSlotInventaire.Count;
        int eqCount = PivotSlotEquipement.Count;

        bool ok = true;
        ok &= Require(scrollbarInventaire != null && scrollbarEquipement != null,
            "Une des deux scrollbars n'est pas assignée.");
        ok &= Require(parentPop != null && nameItem != null && descriptionItem != null && lore != null,
            "parentPop, nameItem, descriptionItem ou lore n'est pas assigné.");
        ok &= Require(SlotInventaire.Count == invCount
                      && RotationInitialSlotInventaire.Count == invCount
                      && isSlotInventaireShow.Count == invCount,
            "Roue inventaire : PivotSlot, Slot, RotationInitial et isSlotShow doivent avoir la même taille.");
        ok &= Require(SlotEquipement.Count == eqCount
                      && RotationInitialItemEquipement.Count == eqCount
                      && isSlotEquipShow.Count == eqCount,
            "Roue équipement : PivotSlot, Slot, RotationInitial et isSlotShow doivent avoir la même taille.");
        ok &= Require(itemEmplacement.Count >= InventoryImageOffset + invCount,
            $"itemEmplacement doit contenir au moins {InventoryImageOffset + invCount} images.");
        return ok;
    }

    private bool Require(bool condition, string message)
    {
        if (!condition) Debug.LogError($"[InventoryWheel] {message}", this);
        return condition;
    }

    private void PadEquipList()
    {
        while (ItemEquip.Count < EquipSlotCount) ItemEquip.Add(null);
    }

    private void CacheSlotBaseRotations()
    {
        slotBaseLocalRotation.Clear();
        for (int i = 0; i < SlotInventaire.Count; i++)
            slotBaseLocalRotation.Add(SlotInventaire[i].localRotation);
    }

    private void InitInventoryIndexes()
    {
        indexItemInventaire.Clear();
        for (int i = 0; i < PivotSlotInventaire.Count; i++)
            indexItemInventaire.Add(i);
    }

    private void UpdateInventoryWheel()
    {
        for (int i = 0; i < PivotSlotInventaire.Count; i++)
        {
            float baseAngle = RotationInitialSlotInventaire[i];
            float angle = ComputeWheelAngle(scrollbarInventaire, vitesseScroll, baseAngle);
            int steps = NormalizeAngle(ref angle, ref baseAngle);
            RotationInitialSlotInventaire[i] = baseAngle;
            indexItemInventaire[i] += steps * InventoryIndexStep;

            SetPivotAngle(PivotSlotInventaire[i], angle);
            RefreshInventorySlotIcon(i);
            UpdateInventorySlotFocus(i, angle);
        }
    }

    private void UpdateInventorySlotFocus(int slot, float angle)
    {
        RectTransform slotRect = SlotInventaire[slot];
        bool inCenter = Mathf.Abs(angle) < popRange;

        if (inCenter)
        {
            slotRect.localScale = popScaleSlotInventaire;
            if (isSlotInventaireShow[slot]) return;

            isSlotInventaireShow[slot] = true;
            itemToShowInventaire = WheelIndexOf(slot);
            slotRect.SetParent(parentPop.transform);
            slotRect.localPosition = Vector3.zero;
            slotRect.localEulerAngles = Vector3.zero;

            RefreshDescription();
        }
        else
        {
            slotRect.localScale = Vector3.one;
            if (!isSlotInventaireShow[slot]) return;

            isSlotInventaireShow[slot] = false;
            slotRect.SetParent(PivotSlotInventaire[slot]);
            slotRect.localPosition = positionBaseSlotInventaire;
            slotRect.localRotation = slotBaseLocalRotation[slot];
        }
    }

    private int WheelIndexOf(int slot)
    {
        int count = ItemInInventory.Count;
        return count == 0 ? -1 : ModuloPositif(indexItemInventaire[slot], count);
    }

    private void UpdateEquipmentWheel()
    {
        for (int i = 0; i < PivotSlotEquipement.Count; i++)
        {
            float baseAngle = RotationInitialItemEquipement[i];
            float angle = ComputeWheelAngle(scrollbarEquipement, vitesseScrollEquip, baseAngle);

            NormalizeAngle(ref angle, ref baseAngle);
            RotationInitialItemEquipement[i] = baseAngle;

            SetPivotAngle(PivotSlotEquipement[i], angle);
            UpdateEquipmentSlotFocus(i, angle);
        }
    }

    private void UpdateEquipmentSlotFocus(int slot, float angle)
    {
        bool inCenter = Mathf.Abs(angle) < popRange;

        SlotEquipement[slot].localScale = inCenter ? popScaleEquip : Vector3.one;
        SlotEquipement[slot].localPosition = inCenter ? Deplacementpop : DeplacementInitial;

        if (!inCenter)
        {
            isSlotEquipShow[slot] = false;
            return;
        }

        if (isSlotEquipShow[slot]) return;

        isSlotEquipShow[slot] = true;
        itemToShowEquipement = slot;
        RefreshDescription();
    }

    private static float ComputeWheelAngle(Scrollbar scrollbar, float speed, float baseAngle)
    {
        return (scrollbar.value - 0.5f) * 360f * speed + baseAngle;
    }

    private static int NormalizeAngle(ref float angle, ref float baseAngle)
    {
        int steps = 0;

        while (angle > HalfStepAngle)
        {
            angle -= StepAngle;
            baseAngle -= StepAngle;
            steps++;
        }

        while (angle < -HalfStepAngle)
        {
            angle += StepAngle;
            baseAngle += StepAngle;
            steps--;
        }

        return steps;
    }

    private static void SetPivotAngle(RectTransform pivot, float angle)
    {
        pivot.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    private static int ModuloPositif(int valeur, int modulo)
    {
        return ((valeur % modulo) + modulo) % modulo;
    }

    private ItemData GetInventoryItem(int index)
    {
        return index >= 0 && index < ItemInInventory.Count ? ItemInInventory[index] : null;
    }

    private ItemData GetEquippedItem(int slot)
    {
        return IsValidEquipSlot(slot) ? ItemEquip[slot] : null;
    }

    private bool IsValidEquipSlot(int slot)
    {
        return slot >= 0 && slot < ItemEquip.Count;
    }

    private void OnItemsChanged()
    {
        SyncItemEmplacement();
        RefreshAllIcons();
        RefreshSelection();
        RefreshDescription();
    }

    private void RefreshSelection()
    {
        int centeredInventorySlot = isSlotInventaireShow.IndexOf(true);
        if (centeredInventorySlot >= 0)
            itemToShowInventaire = WheelIndexOf(centeredInventorySlot);

        int centeredEquipSlot = isSlotEquipShow.IndexOf(true);
        if (centeredEquipSlot >= 0)
            itemToShowEquipement = centeredEquipSlot;
    }

    private void RefreshDescription()
    {
        ItemData item = IsInventory
            ? GetInventoryItem(itemToShowInventaire)
            : GetEquippedItem(itemToShowEquipement);

        nameItem.text = item != null ? item.Name : string.Empty;
        descriptionItem.text = item != null ? item.Description : string.Empty;
        lore.text = item != null ? item.lore : string.Empty;
    }

    private void RefreshAllIcons()
    {
        for (int i = 0; i < PivotSlotInventaire.Count; i++)
            RefreshInventorySlotIcon(i);

        for (int n = 0; n < EquipSlotCount; n++)
            SetIcon(n, GetEquippedItem(n));
    }

    private void RefreshInventorySlotIcon(int slot)
    {
        SetIcon(InventoryImageOffset + slot, GetInventoryItem(WheelIndexOf(slot)));
    }

    private void SetIcon(int imageIndex, ItemData item)
    {
        Image image = itemEmplacement[imageIndex];
        if (image == null) return;

        Sprite sprite = item != null ? item.Icon : null;
        if (sprite == null) sprite = emptySlotSprite;   

        if (image.sprite != sprite) image.sprite = sprite;
    }

    private void SyncItemEmplacement()
    {
        for (int i = 0; i < ItemInInventory.Count; i++)
            if (ItemInInventory[i] != null) ItemInInventory[i].emplacement = i;

        for (int n = 0; n < ItemEquip.Count; n++)
            if (ItemEquip[n] != null) ItemEquip[n].emplacement = n;
    }
}