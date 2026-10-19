using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameScene = "Game";
    
    public void SwitchToGame()
    {
        SceneManager.LoadScene(gameScene);
    }
}
