using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class SubmitRunFonction : MonoBehaviour
{
   [SerializeField] private UIManager forTimer;
   [SerializeField] private Fight forData;
   [SerializeField] private List<int> item = new List<int>();
   [SerializeField] private PlayerHealth forLife;
   private List<int> pallierList = new List<int>();
   private List<float> pallierTimer = new List<float>();
   private float timerA;
   private float timerB;
   private float lastPallierTime;
   
   private void Start()
   {
      ResetPaliers();
      pallierList.Clear();
   }

   public void subRun()
   {
      float timer = forTimer._timer;
      for (int i = 0; i < 5; i++)
      {
         if (PlayerState.Instance.EquippedSkills.Count > i)
         {
            item[i] = PlayerState.Instance.EquippedSkills[i].ID;
         }
         else
         {
            item[i] = -1;
         }
      }
      
      string Lvl = forData.data.title;
      string pallierSkillTime = BuildPallierString();
      int restLife = forLife._health;
      int x = -1;
      switch (Lvl)
      {
         case "1-1" :
            x = 0;
            break;
         case "1-2" :
            x = 1;
            break;
         case "1-3" :
            x = 2;
            break;
         case "2-1" :
            x = 3;
            break;
         case "2-2" :
            x = 4;
            break;
         case "2-3" :
            x = 5;
            break;
      }

      if (timer < PlayerIdentity.Instance.pb[x])
      {
         PlayerIdentity.Instance.pb[x] = timer;
      }
      LevelLeaderboard.SubmitRun(Lvl, timer,item,pallierSkillTime,restLife);
      forTimer._timer=-5;
   }
   private string BuildPallierString()
   {
      StringBuilder sb = new StringBuilder();
      int count = Mathf.Min(pallierList.Count, pallierTimer.Count);

      for (int i = 0; i < count; i++)
      {
         if (i > 0) sb.Append(" ; ");
         sb.Append(pallierList[i])
            .Append(" (")
            .Append(pallierTimer[i].ToString("F2", CultureInfo.InvariantCulture))
            .Append(" )");
      }

      return sb.ToString();
   }

   public void stockPallierSkill(int pallier)
   {
      float now = forTimer._timer;

      pallierList.Add(pallier+1);
      pallierTimer.Add(now - lastPallierTime);
      lastPallierTime = now;

   }
   private void ResetPaliers()
   {
      pallierList.Clear();
      pallierTimer.Clear();
      lastPallierTime = 0f;
   }
}
