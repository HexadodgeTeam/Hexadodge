
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWheel : MonoBehaviour
{
    public List<ItemData> ItemInInventory = new List<ItemData>();
    public List<Image> itemEmplacement = new List<Image>();
    public List<ItemData> ItemEquip = new List<ItemData>();
    
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
    public float vitesseScrollEquip=-10;
    public Vector3 popScaleEquip;
    public float popRangeEquip;

    public Vector3 localPositionForEquip;
    public Vector3 localPositionForInventaire;

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
            }
            
            else 
            {
                SlotEquipement[i].localScale = Vector3.one;
            }
            
            i++;
            if (i >= PivotSlotEquipement.Count) i = 0;
        }
    }
    void RotationInventaire()
    {
        int i = 0;
        foreach (var VARIABLE in  PivotSlotInventaire )
        {
            rotationInventaire.z = (scrollbarInventaire.value-0.5f) * 360f * (vitesseScroll) + RotationInitialSlotInventaire[i];
            PivotSlotInventaire[i].localEulerAngles = new Vector3(0, 0, rotationInventaire.z);
            
            if (rotationInventaire.z > 21) RotationInitialSlotInventaire[i] -= 42;
            else if (rotationInventaire.z < -21) RotationInitialSlotInventaire[i] += 42;

            if (rotationInventaire.z > -popRange && rotationInventaire.z < popRange) 
            {
                SlotInventaire[i].localScale = popScaleSlotInventaire;
                if ( !isSlotInventaireShow[i])
                {
                    isSlotInventaireShow[i] = true;
                    SlotInventaire[i].transform.SetParent(parentPop.transform);
                    SlotInventaire[i].localPosition = Vector3.zero;
                    SlotInventaire[i].localEulerAngles = Vector3.zero;
                }  
            }
            
            else 
            {
                SlotInventaire[i].localScale = Vector3.one;
                if ( isSlotInventaireShow[i]==true)
                {
                    SlotInventaire[i].transform.SetParent(PivotSlotInventaire[i].transform);
                    SlotInventaire[i].localPosition = positionBaseSlotInventaire;
                    isSlotInventaireShow[i] = false;
                } 
            }
            
            i++;
            if (i >= PivotSlotInventaire.Count) i = 0;
        }
    }

    public void SwapItem()
    {
        
        int k;
        int l;
        int m;
        k = 0;
        l = 0;
        m = 0;
        for (int i = 0; i < PivotSlotInventaire.Count; i++)
        {
            float rotationPivot = Mathf.DeltaAngle(
                0f,
                PivotSlotInventaire[i].localEulerAngles.z
            );

            if (Mathf.Abs(rotationPivot) < popRange)
            {
                k = i;
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

        ItemData Temp = ItemEquip[l];

        if (ItemInInventory[k] == null) return;

        ItemEquip.Insert(l, ItemInInventory[k]);
        ItemEquip.RemoveAt(l + 1);

        if (Temp == null)
        {
            ItemInInventory.RemoveAt(k);
            UpdateItem();
            return;
        }

        ItemInInventory.Insert(k, Temp);
        ItemInInventory.RemoveAt(k + 1);

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
}