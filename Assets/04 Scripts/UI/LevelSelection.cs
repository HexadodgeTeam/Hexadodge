
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
   
    [SerializeField] private List<RuntimeAnimatorController> enemyAnim = new();
    [SerializeField] private List<GameObject> enemy = new();
    [SerializeField] private List<FightData> fights = new();
   
    public int currentLevel = 0;
    public int previousLevel = 0;

    public void ChangeSpecificLevel(int lvl)
    {
        previousLevel =  currentLevel;
        currentLevel = lvl;
        Animator anim1 = enemy[1].GetComponent<Animator>();
        anim1.runtimeAnimatorController = enemyAnim[currentLevel];
    }
    public void ChangeLevel(int lvl)
    {
        currentLevel += lvl;

        if (currentLevel >= enemyAnim.Count)
            currentLevel = 0;

        if (currentLevel < 0)
            currentLevel = enemyAnim.Count - 1;

        if (lvl > 0)
        {
            Animator anim1 = enemy[1].GetComponent<Animator>();
            anim1.SetTrigger("SG");

            Animator anim2 = enemy[2].GetComponent<Animator>();
            anim2.runtimeAnimatorController = enemyAnim[currentLevel];
            anim2.SetTrigger("ED");

            StartCoroutine(ChangeEnemy());
        }
        else if (lvl < 0)
        {
            Animator anim1 = enemy[1].GetComponent<Animator>();
            anim1.SetTrigger("SD");

            Animator anim2 = enemy[0].GetComponent<Animator>();
            anim2.runtimeAnimatorController = enemyAnim[currentLevel];
            anim2.SetTrigger("EG");

            StartCoroutine(ChangeEnemy());
        }
    }

    IEnumerator ChangeEnemy()
    {
        yield return new WaitForSeconds(1f);

        Animator anim1 = enemy[1].GetComponent<Animator>();
        anim1.runtimeAnimatorController = enemyAnim[currentLevel];
        yield return null;
    }

    public void SetFight()
    {
        FightDataTransfer.Instance.SetData(fights[currentLevel < fights.Count ? currentLevel : 0]);
    }
}