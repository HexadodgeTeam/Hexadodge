using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class SubmitRunFonction : MonoBehaviour
{
   [SerializeField] private UIManager forTimer;
   [SerializeField] private Fight forData;
   [SerializeField] private Game forGameData;
   [SerializeField] private List<int> item = new List<int>();
   public void subRun()
   {
      float timer = forTimer._timer;
      for (int i = 0; i < 5; i++)
      {
         if (forGameData.data.baseSkills.Length>i)
         {
            
            item[i] = forGameData.data.baseSkills[i].ID;
         }
         else
         {
            item[i] = -1;
         }
      }
      
      string Lvl = forData.data.title;
      LevelLeaderboard.SubmitRun(Lvl, timer,item);
      forTimer._timer=-5;
   }
}
