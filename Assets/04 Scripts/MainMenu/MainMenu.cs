using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameScene = "Game";
    [SerializeField] private float launchGameDelay = 1f;
    [SerializeField] private FightData tutorialFight;
    [SerializeField] private GameObject setPseudo;
    [SerializeField] private SkillRewardMenu skillRewardMenu;
    
    private async void Start()
    {
        try
        {
            await PlayerIdentity.Init();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return;
        }

        if (this == null) return;

        string playerName = PlayerIdentity.GetPseudo();
        if (playerName == "Joueur")
        {
            PlayerIdentity.SetPseudo("JoueurET");
            FightDataTransfer.Instance.SetData(tutorialFight);
            SwitchToGame();
        }
        else if (playerName == "JoueurET")
        {
            setPseudo.SetActive(true);
        }
        
        HandleSkillRewards();
    }

    private void HandleSkillRewards()
    {
        if (PlayerState.Instance.UpcomingSkills.Count <= 0) return;
        
        skillRewardMenu.Open();
    }

    public void SwitchToGame()
    {
        StartCoroutine(LaunchGame());
    }

    private IEnumerator LaunchGame()
    {
        yield return new WaitForSeconds(launchGameDelay);
        
        SceneManager.LoadScene(gameScene);
    }
}
