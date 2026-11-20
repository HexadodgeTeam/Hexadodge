using UnityEngine;

public class SettAttemps : MonoBehaviour
{
  public FightDataTransfer fightData;
  public void SetAttempsFonction()
  {
    LevelLeaderboard.OnLevelStart(fightData.fightData.title);
  }
}
