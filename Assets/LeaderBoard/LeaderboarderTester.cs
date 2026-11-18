using System.Collections.Generic;
using UnityEngine;
using TaloGameServices;

public class LeaderboardTester : MonoBehaviour
{
    [Header("À assigner")]
    public LevelLeaderboard leaderboard;
    public LeaderboardView[] views;
    
    public IList<int> testItems = new List<int> { -1, -1, -1, 2, -1 };

    public async void RunTest()
    {
        LevelLeaderboard.SubmitRun("1-1", 21, testItems);
        views[0].Refresh();
    }
    
}