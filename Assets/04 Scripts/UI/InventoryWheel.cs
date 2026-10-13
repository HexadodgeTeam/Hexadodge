
using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWheel : MonoBehaviour
{
    public Scrollbar scrollbar;
    public RectTransform chapeau;
    private Quaternion rotation;
    public float vitesseScroll=-10;

    private void Update()
    {
       
        rotation.z = (scrollbar.value) * 360f*(vitesseScroll);
        chapeau.localEulerAngles = new Vector3(0, 0, rotation.z);
    }
}