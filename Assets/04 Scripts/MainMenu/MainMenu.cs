using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameScene = "Game";
    [SerializeField] private float launchGameDelay = 1f;
    [SerializeField] private LevelSelection levelSelection;
    
    public void SwitchToGame()
    {
        StartCoroutine(LaunchGame());
    }

    private IEnumerator LaunchGame()
    {
        yield return new WaitForSeconds(launchGameDelay);
        
        SceneManager.LoadScene(levelSelection.GetScene());
    }
}
