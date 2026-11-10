
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class LevelSelection : MonoBehaviour
{
   
    [SerializeField] private List<RuntimeAnimatorController> enemyAnim = new();
    [SerializeField] private List<GameObject> enemyanimator = new();
    [SerializeField] private GameObject BG ;
    [SerializeField] private List<GameObject> enemy = new();
    [SerializeField] private List<FightData> fights = new();
    [SerializeField] private float changeSpeed;
    [SerializeField] private List<Vector2> enemytransform = new();
    [SerializeField] private TextMeshProUGUI ennemyName;
    [SerializeField] private List<string> sceneName;
    private bool  isChanging = false;
    public int currentLevel = 0;
    public int previousLevel = 0;
    public bool isOpen=false;
    public Vector2 startTouchPosition;
    public Vector2 endTouchPosition;
    public int actualFloor = 0;
    private Animator anim0;
    private Animator anim1;
    private Animator anim2;

    private Animator anim3;

    private Animator anim4;

    public string GetScene()
    {
        return sceneName[currentLevel];
    }

    private void Update()
    {
        if (!isOpen)
        {
            Swipe();
        }
    }

    void Swipe()
    {
        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            startTouchPosition = touch.position.ReadValue();
        }

        if (touch.press.wasReleasedThisFrame)
        {
            endTouchPosition = touch.position.ReadValue();

            Vector2 inputVector = endTouchPosition - startTouchPosition;

            if (inputVector.magnitude < 50f)
                return;

            if (Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.y))
            {
                if (inputVector.x > 0)
                    RightSwipe();
                else
                    LeftSwipe();
            }
            else
            {
                if (inputVector.y > 0)
                    UpSwipe();
                else
                    DownSwipe();
            }
        }
    }

    private void UpSwipe()
    {
        ChangeFloorL(-1);
    }
    private void DownSwipe()
    {
        ChangeFloorL(1);
    }
    private void LeftSwipe()
    {
        ChangeLevel(1);
    }
    private void RightSwipe()
    {
        ChangeLevel(-1);
    }



    private void Start()
    {
        isOpen = false;
        enemytransform.Clear();
        ennemyName.text = fights[currentLevel < fights.Count ? currentLevel : 0].title;
        foreach (GameObject e in enemy)
        {
            enemytransform.Add(e.GetComponent<RectTransform>().anchoredPosition);
        }
        anim0 = enemyanimator[0].GetComponent<Animator>();
        anim1 = enemyanimator[1].GetComponent<Animator>();
        anim2 = enemyanimator[2].GetComponent<Animator>();
        
        anim3 = enemyanimator[3].GetComponent<Animator>();
        anim4 = enemyanimator[4].GetComponent<Animator>();
    }

    public void ChangeSpecificLevel(int lvl)
    {
        if (isChanging || lvl < 0 || lvl >= enemyAnim.Count)
            return;
        
        previousLevel = currentLevel;
        currentLevel = lvl;
        
        int quotient = currentLevel / 3;
        actualFloor = quotient ;
        BG.transform.position = new Vector3(0, -(actualFloor ) * 11, 0);

        Animator anim = enemyanimator[0].GetComponent<Animator>();

        if (anim != null)
            anim.runtimeAnimatorController = enemyAnim[currentLevel];
        ennemyName.text = fights[currentLevel < fights.Count ? currentLevel : 0].title;
    }

    public void ChangeLevel(int lvl)
    {
        if (isChanging || lvl == 0 || enemyAnim.Count == 0)
            return;

        if (enemy.Count < 3)
            return;

        if (lvl < 0 && currentLevel % 3 == 0) currentLevel += 2;
        else if (lvl>0 && currentLevel % 3 == 2) currentLevel -= 2;
        else currentLevel += lvl > 0 ? 1 : -1;
        
        previousLevel = currentLevel;

        

        if (currentLevel >= enemyAnim.Count)
            currentLevel = 0;

        if (currentLevel < 0)
            currentLevel = enemyAnim.Count - 1;

        StartCoroutine(ChangeEnemy(lvl));
    }
    public void ChangeFloorL(int lvl)
    {
        if (isChanging || lvl == 0 || enemyAnim.Count == 0)
            return;

        if (enemy.Count < 3)
            return;
        if (lvl<0 && currentLevel - 3 < 0)
            return;

        actualFloor += lvl;
        previousLevel = currentLevel;

        currentLevel += lvl > 0 ? 3-(currentLevel%3) : -3-(currentLevel%3);

        if (currentLevel >= enemyAnim.Count)
            currentLevel = 0;

        

        StartCoroutine(ChangeFloor(lvl));
    }
    private IEnumerator ChangeFloor(int direction)
    {
        isChanging = true;

        
        Vector2 startCenter = enemytransform[1];
        Vector2 startMoving = enemytransform[direction > 0 ? 4 : 3];

        Vector2 endCenter = enemytransform[direction > 0 ? 3 : 4];
        Vector2 endMoving = enemytransform[1];

        float elapsed = 0f;
        float duration = changeSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            enemy[1].GetComponent<RectTransform>().anchoredPosition =
                Vector2.Lerp(startCenter, endCenter, t);

            enemy[direction > 0 ? 3 : 4].GetComponent<RectTransform>().anchoredPosition =
                Vector2.Lerp(startMoving, endMoving, t);
            int x = -11 * (actualFloor);
            
            Vector2 objectif = new Vector2(0, x);
            
            BG.transform.position = Vector2.Lerp(BG.transform.position, objectif, t) ;
            yield return null;
        }
       
        
        SetOriginPosition();
        
        anim3.runtimeAnimatorController = enemyAnim[Math.Abs(currentLevel+3)-(currentLevel%3)%enemyAnim.Count];
        anim4.runtimeAnimatorController = enemyAnim[Math.Abs(currentLevel-3)-(currentLevel%3)%enemyAnim.Count];
        anim0.runtimeAnimatorController = enemyAnim[(currentLevel)%enemyAnim.Count];
        ennemyName.text = fights[currentLevel < fights.Count ? currentLevel : 0].title;

       
        isChanging = false;
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
       
       
        
        
        yield return  new WaitForSeconds(0.2f);
        SetOriginPosition();
        anim0.runtimeAnimatorController = enemyAnim[((currentLevel)%3)+(actualFloor*3)];
        anim2.runtimeAnimatorController = enemyAnim[Math.Abs(currentLevel-1)%3+(actualFloor*3)];
        anim1.runtimeAnimatorController = enemyAnim[(currentLevel+1)%3+(actualFloor*3)];
        ennemyName.text = fights[currentLevel < fights.Count ? currentLevel : 0].title;

        
       
        isChanging = false;
    }

    public void SetOriginPosition()
    {
        enemy[0].GetComponent<RectTransform>().anchoredPosition = enemytransform[0];
        enemy[1].GetComponent<RectTransform>().anchoredPosition = enemytransform[1];
        enemy[2].GetComponent<RectTransform>().anchoredPosition = enemytransform[2];
        enemy[3].GetComponent<RectTransform>().anchoredPosition = enemytransform[3];
        enemy[4].GetComponent<RectTransform>().anchoredPosition = enemytransform[4];
    }

    public void SetBoolInMenu(bool Open)
    {
       // isOpen = Open;
    }


    public void SetFight()
    {
        FightDataTransfer.Instance.SetData(fights[currentLevel < fights.Count ? currentLevel : 0]);
    }
}