using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text gameOverField;

    public void Display(bool win)
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
        
        gameOverField.text = win ? "You win!" : "You lose!";
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
