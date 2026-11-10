using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBeforeActive : MonoBehaviour
{
    [SerializeField] private GameObject animGameObject;
    [SerializeField] private Animator animToWaitFor;
    [SerializeField] private float dureeAnimation;
    [SerializeField] private GameObject uiToActive;
    [SerializeField] private LevelSelection levelSelection;
    public void StartWaitForAnim(bool forOpen)
    {
        StartCoroutine(WaitForAnim(forOpen));
    }

    public IEnumerator WaitForAnim(bool forOpen)
    {
        if (!forOpen)
        {
            
            uiToActive.SetActive(false);
            animGameObject.SetActive(true);
            animToWaitFor.SetTrigger("Close");
            yield return new WaitForSeconds(dureeAnimation);
            animGameObject.SetActive(false);
            levelSelection.isOpen=false;
        }
        else
        {
            levelSelection.isOpen=true;
            animGameObject.SetActive(true);
            animToWaitFor.SetTrigger("Open");
            yield return new WaitForSeconds(dureeAnimation);
            animGameObject.SetActive(false);
            uiToActive.SetActive(true);  
        }
    }
}
