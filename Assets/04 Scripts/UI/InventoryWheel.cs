
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWheel : MonoBehaviour
{
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
            i++;
        }
        int j = 0;
        foreach (var VARIABLE in  PivotSlotEquipement )
        {
            var angles = PivotSlotEquipement[j].localEulerAngles;
            angles.z = RotationInitialItemEquipement[j]   ;
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
        int i = 0;
        int k = 0;
        int l;
        foreach (var VARIABLE in  PivotSlotInventaire )
        {
            if (PivotSlotInventaire[i].localEulerAngles.z > -popRange && PivotSlotInventaire[i].localEulerAngles.z < popRange )
            {
                k = i;
                break;
            }
            
        }
        int j = 0;
        foreach (var VARIABLE in  PivotSlotEquipement )
        {
            if (PivotSlotEquipement[i].localEulerAngles.z > -popRangeEquip && PivotSlotEquipement[i].localEulerAngles.z < popRange )
            {
                l = j;
                break;
            }
        }

        SlotInventaire[k].transform.SetParent(PivotSlotEquipement[j].transform) ;
        SlotInventaire[k].localPosition = localPositionForEquip;
        SlotEquipement[j].transform.SetParent(PivotSlotInventaire[k].transform) ;
        SlotEquipement[j].localPosition = localPositionForInventaire;
    }
}