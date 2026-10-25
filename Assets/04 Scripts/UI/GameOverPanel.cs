using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : UIPanel
{
    [SerializeField] private TMP_Text gameOverField;

    public void Display(bool win)
    {
        Open();
        
        gameOverField.text = win ? "You win!" : "You lose!";
    }

    public void Restart()
    {
        Close();
        
        Game.Instance.SetTransfer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
