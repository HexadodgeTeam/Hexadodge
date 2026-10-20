using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaOnMap : MonoBehaviour
{
    [SerializeField]private RectTransform player;
    [SerializeField]private List<RectTransform> mapPoint = new List<RectTransform>();
    [SerializeField]private LevelSelection levelSelection;
    [SerializeField]private List<RectTransform> transitionList = new List<RectTransform>();
    
    float moveDuration = 1f;

    private void OnEnable()
    {
        UpdateCharPosition();
    }

    public void UpdateCharPosition()
{
    int serieActuel = levelSelection.previousLevel / 3;
    int serieTarget = levelSelection.currentLevel / 3;
    int LvlActuel = levelSelection.previousLevel % 3;
    int LvlTarget = levelSelection.currentLevel % 3;
    if (serieActuel == serieTarget || (LvlActuel == LvlTarget && LvlActuel== 0))
    {
        StartCoroutine(MovePlayer( player.anchoredPosition, mapPoint[levelSelection.currentLevel].anchoredPosition,null,null));
    }
    else
    {
        StartCoroutine(MovePlayer(player.anchoredPosition,mapPoint[levelSelection.currentLevel].anchoredPosition,transitionList[serieActuel].anchoredPosition,transitionList[serieTarget].anchoredPosition));
    }
}

private IEnumerator MovePlayer(
    Vector2 startPosition,
    Vector2 targetPosition,
    Vector2? transition1,
    Vector2? transition2)
{
    
    if (!transition1.HasValue || !transition2.HasValue)
    {
        yield return StartCoroutine(LerpPosition(startPosition, targetPosition, moveDuration));
        yield break;
    }

    yield return StartCoroutine(LerpPosition(startPosition, transition1.Value, moveDuration / 3f));
    yield return StartCoroutine(LerpPosition(transition1.Value, transition2.Value, moveDuration / 3f));
    yield return StartCoroutine(LerpPosition(transition2.Value, targetPosition, moveDuration / 3f));
}

private IEnumerator LerpPosition(Vector2 startPosition,Vector2 targetPosition,float duration)
    {
    float timer = 0f;

    while (timer < duration)
    {
        timer += Time.deltaTime;

        float t = timer / duration;
        t = Mathf.SmoothStep(0f, 1f, t);

        player.anchoredPosition = Vector2.Lerp(startPosition,targetPosition,t);
        yield return null;
    }

    player.anchoredPosition = targetPosition;
    }
}
