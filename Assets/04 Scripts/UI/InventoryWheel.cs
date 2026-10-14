
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWheel : MonoBehaviour
{
    public Scrollbar scrollbar;
    public List<RectTransform> chapeauItem = new List<RectTransform>();
    public List<RectTransform> EnfantItem = new List<RectTransform>();
    public List<float> RotationInitialItem = new List<float>();
    public List<bool> isShow = new List<bool>();
    private Quaternion rotation;
    public float vitesseScroll=-10;
    public Vector3 popScale;
    public Vector3 positionBase;
    public bool isEquipement;
    public GameObject parentPop;
    public float popRange;

    private void Start()
    {
        int i = 0;
        foreach (var VARIABLE in  chapeauItem )
        {
            var angles = chapeauItem[i].localEulerAngles;
            angles.z = RotationInitialItem[i]   ;
            chapeauItem[i].localEulerAngles = angles;
            i++;
        }
    }

    private void Update()
    {
        RotationChapeau();
        
    }

    void RotationChapeau()
    {
        int i = 0;
        foreach (var VARIABLE in  chapeauItem )
        {
            rotation.z = (scrollbar.value-0.5f) * 360f * (vitesseScroll) + RotationInitialItem[i];
            chapeauItem[i].localEulerAngles = new Vector3(0, 0, rotation.z);
            
            if (rotation.z > 21) RotationInitialItem[i] -= 42;
            else if (rotation.z < -21) RotationInitialItem[i] += 42;

            if (rotation.z > -popRange && rotation.z < popRange) 
            {
                EnfantItem[i].localScale = popScale;
                if (!isEquipement &&  !isShow[i])
                {
                    isShow[i] = true;
                    EnfantItem[i].transform.SetParent(parentPop.transform);
                    EnfantItem[i].localPosition = Vector3.zero;
                    EnfantItem[i].localEulerAngles = Vector3.zero;
                }  
            }
            
            else 
            {
                EnfantItem[i].localScale = Vector3.one;
                if (!isEquipement && isShow[i]==true)
                {
                    EnfantItem[i].transform.SetParent(chapeauItem[i].transform);
                    EnfantItem[i].localPosition = positionBase;
                    isShow[i] = false;
                } 
            }
            
            i++;
            if (i >= chapeauItem.Count) i = 0;
        }
    }
}