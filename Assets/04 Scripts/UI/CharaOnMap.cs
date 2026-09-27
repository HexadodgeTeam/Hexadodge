using System;
using System.Collections.Generic;
using UnityEngine;

public class CharaOnMap : MonoBehaviour
{
    [SerializeField]private RectTransform player;
    [SerializeField]private List<RectTransform> mapPoint = new List<RectTransform>();
    [SerializeField]private LevelSelection levelSelection;

    private void OnEnable()
    {
        player.position = mapPoint[levelSelection.actualLvl].position;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
