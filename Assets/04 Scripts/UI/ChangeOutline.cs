using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOutline : MonoBehaviour
{
    [SerializeField] private Image sprite;
    [SerializeField] private List<Material> outlineType = new List<Material>();
    private int i = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeOutlineFonction()
    {
        i += 1;
        if (i >= outlineType.Count) i = 0;
        sprite.material = outlineType[i];
    }
}
