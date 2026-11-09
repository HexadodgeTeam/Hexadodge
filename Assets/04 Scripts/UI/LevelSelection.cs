
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
   
    [SerializeField] private List<RuntimeAnimatorController> enemyAnim = new();
    [SerializeField] private List<GameObject> enemyanimator = new();
    [SerializeField] private List<GameObject> enemy = new();
    [SerializeField] private List<FightData> fights = new();
    [SerializeField] private float changeSpeed;
    [SerializeField] private List<Vector2> enemytransform = new();
    [SerializeField] private TextMeshProUGUI ennemyName;
    private bool  isChanging = false;
    public int currentLevel = 0;
    public int previousLevel = 0;

    private void Start()
    {
        enemytransform.Clear();
        ennemyName.text = fights[currentLevel < fights.Count ? currentLevel : 0].title;
        foreach (GameObject e in enemy)
        {
            enemytransform.Add(e.GetComponent<RectTransform>().anchoredPosition);
        }
    }

    public void ChangeSpecificLevel(int lvl)
    {
        if (isChanging || lvl < 0 || lvl >= enemyAnim.Count)
            return;

        previousLevel = currentLevel;
        currentLevel = lvl;

        Animator anim = enemyanimator[1].GetComponent<Animator>();

        if (anim != null)
            anim.runtimeAnimatorController = enemyAnim[currentLevel];
    }

    public void ChangeLevel(int lvl)
    {
        if (isChanging || lvl == 0 || enemyAnim.Count == 0)
            return;

        if (enemy.Count < 3)
            return;

        previousLevel = currentLevel;

        currentLevel += lvl > 0 ? 1 : -1;

        if (currentLevel >= enemyAnim.Count)
            currentLevel = 0;

        if (currentLevel < 0)
            currentLevel = enemyAnim.Count - 1;

        StartCoroutine(ChangeEnemy(lvl));
    }

    private IEnumerator ChangeEnemy(int direction)
    {
        isChanging = true;

        
        Vector2 startCenter = enemytransform[1];
        Vector2 startMoving = enemytransform[direction > 0 ? 2 : 0];

        Vector2 endCenter = enemytransform[direction > 0 ? 0 : 2];
        Vector2 endMoving = enemytransform[1];

        float elapsed = 0f;
        float duration = changeSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            enemy[1].GetComponent<RectTransform>().anchoredPosition =
                Vector2.Lerp(startCenter, endCenter, t);

            enemy[direction > 0 ? 2 : 0].GetComponent<RectTransform>().anchoredPosition =
                Vector2.Lerp(startMoving, endMoving, t);
            yield return null;
        }
       
        Animator anim0 = enemyanimator[0].GetComponent<Animator>();
        Animator anim1 = enemyanimator[1].GetComponent<Animator>();
        Animator anim2 = enemyanimator[2].GetComponent<Animator>();
        
        anim0.runtimeAnimatorController = enemyAnim[(currentLevel)%enemyAnim.Count];
        anim2.runtimeAnimatorController = enemyAnim[Math.Abs(currentLevel-1)%enemyAnim.Count];
        anim1.runtimeAnimatorController = enemyAnim[(currentLevel+1)%enemyAnim.Count];
        ennemyName.text = fights[currentLevel < fights.Count ? currentLevel : 0].title;

        SetOriginPosition();
       
        isChanging = false;
    }

    public void SetOriginPosition()
    {
        enemy[0].GetComponent<RectTransform>().anchoredPosition = enemytransform[0];
        enemy[1].GetComponent<RectTransform>().anchoredPosition = enemytransform[1];
        enemy[2].GetComponent<RectTransform>().anchoredPosition = enemytransform[2];
    }

    public void SetFight()
    {
        FightDataTransfer.Instance.SetData(fights[currentLevel < fights.Count ? currentLevel : 0]);
    }
}