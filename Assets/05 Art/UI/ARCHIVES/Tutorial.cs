using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> Tuto;
    void Start()
    {
        StartCoroutine(_TutorialCoroutine());
    }

    public  IEnumerator _TutorialCoroutine()
    {
        yield return new WaitForSecondsRealtime(4f);
        Tuto[0].SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
        Tuto[0].SetActive(false);
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 0f;
        Tuto[1].SetActive(true);
        yield return new WaitForSecondsRealtime(7f);
        Tuto[1].SetActive(false);
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(5f);
        Tuto[2].SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        Tuto[2].SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
