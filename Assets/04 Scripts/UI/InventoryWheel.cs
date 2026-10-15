
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWheel : MonoBehaviour
{
    public List<ItemData> ItemInInventory = new List<ItemData>();
    public List<Image> itemEmplacement = new List<Image>();
    public List<ItemData> ItemEquip = new List<ItemData>();

    public bool IsInventory;
    
    public Scrollbar scrollbarInventaire;
    public List<RectTransform> PivotSlotInventaire = new List<RectTransform>();
    public List<RectTransform> SlotInventaire = new List<RectTransform>();
    public List<float> RotationInitialSlotInventaire = new List<float>();
    public List<bool> isSlotInventaireShow = new List<bool>();
    private Quaternion rotationInventaire;
    public float vitesseScroll=-10;
    public Vector3 popScaleSlotInventaire;
    public Vector3 positionBaseSlotInventaire;
    public GameObject parentPop;
    public float popRange;
    
    
    
    public Scrollbar scrollbarEquipement;
    public List<RectTransform> PivotSlotEquipement = new List<RectTransform>();
    public List<RectTransform> SlotEquipement = new List<RectTransform>();
    public List<float> RotationInitialItemEquipement = new List<float>();
    private Quaternion rotationEquipement;
    public List<bool> isSlotEquipShow = new List<bool>();
    public float vitesseScrollEquip=-10;
    public Vector3 popScaleEquip;
    public float popRangeEquip;

    public TextMeshProUGUI nameItem;
    public TextMeshProUGUI descriptionItem;
    public TextMeshProUGUI lore;
    public int itemToShowInventaire;
    public int itemToShowEquipement;

    public Vector3 localPositionForEquip;
    public Vector3 localPositionForInventaire;
    
    public List<int> indexItemInventaire = new List<int>();

    public void changeDesciptionFocus(bool isInventoryB)
    {
        IsInventory = isInventoryB;
        UpdateInventoryDescription();
    }

    void UpdateInventoryDescription()
    {
        int i = 0;
        if (IsInventory)
        {
            foreach (var VARIABLE in ItemInInventory)
            {
                if (ItemInInventory[i].emplacement == itemToShowInventaire)
                {
                    nameItem.text = ItemInInventory[i].Name;
                    descriptionItem.text = ItemInInventory[i].Description;
                    lore.text = ItemInInventory[i].lore;
                }
                else  i++;
            }
        }
        else
        {

            foreach (var VARIABLE in ItemEquip)
            {
                if (ItemEquip[i].emplacement == itemToShowEquipement)
                {
                    nameItem.text = ItemEquip[i].Name;
                    descriptionItem.text = ItemEquip[i].Description;
                    lore.text = ItemEquip[i].lore;
                }
                else  i++;
            }
        }
    }

    private void Start()
    {
        int i = 0;
        foreach (var VARIABLE in  PivotSlotInventaire )
        {
            var angles = PivotSlotInventaire[i].localEulerAngles;
            angles.z = RotationInitialSlotInventaire[i]   ;
            PivotSlotInventaire[i].localEulerAngles = angles;

            itemEmplacement[i+5].sprite = ItemInInventory[i].Icon;
            ItemInInventory[i].emplacement = i;
            
            i++;
        }
        int j = 0;
        foreach (var VARIABLE in  PivotSlotEquipement )
        {
            var angles = PivotSlotEquipement[j].localEulerAngles;
            angles.z = RotationInitialItemEquipement[j];
            PivotSlotEquipement[j].localEulerAngles = angles;
            j++;
        }
        for (int k = 0; k < PivotSlotInventaire.Count; k++)
        {
            indexItemInventaire.Add(k % ItemInInventory.Count);
        }
    }

    private void Update()
    {
        RotationInventaire();
        RotationEquipement();

    }

    void RotationEquipement()
    {
        int i = 0;
        foreach (var VARIABLE in  PivotSlotEquipement )
        {
            rotationEquipement.z = (scrollbarEquipement.value-0.5f) * 360f * (vitesseScrollEquip) + RotationInitialItemEquipement[i];
            PivotSlotEquipement[i].localEulerAngles = new Vector3(0, 0, rotationEquipement.z);
            
            if (rotationEquipement.z > 21) RotationInitialItemEquipement[i] -= 42;
            else if (rotationEquipement.z < -21) RotationInitialItemEquipement[i] += 42;

            if (rotationEquipement.z > -popRange && rotationEquipement.z < popRange) 
            {
                SlotEquipement[i].localScale = popScaleEquip;
                if (!isSlotEquipShow[i])
                {
                    isSlotEquipShow[i] = true;
                    itemToShowEquipement = i;
                    UpdateInventoryDescription();
                }
            }
            
            else 
            {
                if (isSlotEquipShow[i] == true)
                {
                    isSlotEquipShow[i] = false;
                }

                SlotEquipement[i].localScale = Vector3.one;
            }
            
            i++;
            if (i >= PivotSlotEquipement.Count) i = 0;
        }
    }
    int ModuloPositif(int valeur, int modulo)
    {
        return ((valeur % modulo) + modulo) % modulo;
    }
    void RotationInventaire()
{
    for (int i = 0; i < PivotSlotInventaire.Count; i++)
    {
        float angle =
            (scrollbarInventaire.value - 0.5f)
            * 360f * vitesseScroll
            + RotationInitialSlotInventaire[i];
        
        while (angle > 21f)
        {
            RotationInitialSlotInventaire[i] -= 42f;
            indexItemInventaire[i] += 8;

            angle -= 42f;
        }

        while (angle < -21f)
        {
            RotationInitialSlotInventaire[i] += 42f;
            indexItemInventaire[i] -= 8;

            angle += 42f;
        }
        
        PivotSlotInventaire[i].localEulerAngles =
            new Vector3(
                0f,
                0f,
                angle
            );

        int index = ModuloPositif(
            indexItemInventaire[i],
            ItemInInventory.Count
        );

        itemEmplacement[i + 5].sprite =
            ItemInInventory[index].Icon;

        if (angle > -popRange && angle < popRange)
        {
            SlotInventaire[i].localScale = popScaleSlotInventaire;

            if (!isSlotInventaireShow[i])
            {
                isSlotInventaireShow[i] = true;
                itemToShowInventaire = index;

                SlotInventaire[i].transform.SetParent(parentPop.transform);
                SlotInventaire[i].localPosition = Vector3.zero;
                SlotInventaire[i].localEulerAngles = Vector3.zero;

                UpdateInventoryDescription();
            }
        }
        else
        {
            SlotInventaire[i].localScale = Vector3.one;

            if (isSlotInventaireShow[i])
            {
                SlotInventaire[i].transform.SetParent(
                    PivotSlotInventaire[i].transform
                );

                SlotInventaire[i].localPosition =
                    positionBaseSlotInventaire;

                isSlotInventaireShow[i] = false;
            }
        }
    }
}

    public void SwapItem()
    {
        
        int k;
        int l;
        
        k = 0;
        l = 0;
        
        for (int i = 0; i < PivotSlotInventaire.Count; i++)
        {
            float rotationPivot = Mathf.DeltaAngle(
                0f,
                PivotSlotInventaire[i].localEulerAngles.z
            );

            if (Mathf.Abs(rotationPivot) < popRange)
            {
                
                k = (int)(i+(RotationInitialSlotInventaire[i] / 42) * 8);
                break;
            }
        }

        for (int j = 0; j < PivotSlotEquipement.Count; j++)
        {
            float rotationPivot = Mathf.DeltaAngle(
                0f,
                PivotSlotEquipement[j].localEulerAngles.z
            );

            if (Mathf.Abs(rotationPivot) < popRangeEquip)
            {
                l = j;
                break;
            }
        }

        ItemData Temp = ItemEquip[itemToShowEquipement];

        if (ItemInInventory[itemToShowInventaire] == null) return;

        ItemEquip.Insert(itemToShowEquipement, ItemInInventory[itemToShowInventaire]);
        ItemEquip.RemoveAt(itemToShowEquipement + 1);

        if (Temp == null)
        {
            ItemInInventory.RemoveAt(itemToShowInventaire);
            UpdateItem();
            return;
        }

        ItemInInventory.Insert(itemToShowInventaire, Temp);
        ItemInInventory.RemoveAt(itemToShowInventaire + 1);

        UpdateItem();

    }

    private void UpdateItem()
    {
        for (int o = 0; o < PivotSlotInventaire.Count; o++)
        {
            int indexImage = o + 5;

            if (o >= ItemInInventory.Count || ItemInInventory[o] == null)
            {
                itemEmplacement[indexImage].sprite = null;
                continue;
            }

            itemEmplacement[indexImage].sprite = ItemInInventory[o].Icon;
            ItemInInventory[o].emplacement = o;
        }
        for (int n = 0; n < PivotSlotEquipement.Count-1; n++)
        {
            if (n >= ItemEquip.Count || ItemEquip[n] == null)
            {
                itemEmplacement[n].sprite = null;
                continue;
            }

            itemEmplacement[n].sprite = ItemEquip[n].Icon;
            ItemEquip[n].emplacement = n;
        }
    }

    public void RemoveItem()
    {
        ItemInInventory.Insert(ItemEquip[itemToShowEquipement].ID,ItemEquip[itemToShowEquipement] );
        ItemEquip.RemoveAt(itemToShowEquipement);

        UpdateItem();
    }
}