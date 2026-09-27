using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
   
    [SerializeField] private List<RuntimeAnimatorController> enemyAnim = new List<RuntimeAnimatorController>();
    [SerializeField] private List<GameObject> enemy = new List<GameObject>();
   
    public int actualLvl = 0;
    public int precedentlLvl = 0;

    public void ChangeSpecificLevel(int lvl)
    {
        precedentlLvl =  actualLvl;
        actualLvl = lvl;
        Animator anim1 = enemy[1].GetComponent<Animator>();
        anim1.runtimeAnimatorController = enemyAnim[actualLvl];
    }
    public void ChangeLevel(int lvl)
    {
        actualLvl += lvl;

        if (actualLvl >= enemyAnim.Count)
            actualLvl = 0;

        if (actualLvl < 0)
            actualLvl = enemyAnim.Count - 1;

        if (lvl > 0)
        {
            Animator anim1 = enemy[1].GetComponent<Animator>();
            anim1.SetTrigger("SG");

            Animator anim2 = enemy[2].GetComponent<Animator>();
            anim2.runtimeAnimatorController = enemyAnim[actualLvl];
            anim2.SetTrigger("ED");

            StartCoroutine(ChangeEnemy());
        }
        else if (lvl < 0)
        {
            Animator anim1 = enemy[1].GetComponent<Animator>();
            anim1.SetTrigger("SD");

            Animator anim2 = enemy[0].GetComponent<Animator>();
            anim2.runtimeAnimatorController = enemyAnim[actualLvl];
            anim2.SetTrigger("EG");

            StartCoroutine(ChangeEnemy());
        }
    }

    IEnumerator ChangeEnemy()
    {
        yield return new WaitForSeconds(1f);

        Animator anim1 = enemy[1].GetComponent<Animator>();
        anim1.runtimeAnimatorController = enemyAnim[actualLvl];
        yield return null;
    }
}