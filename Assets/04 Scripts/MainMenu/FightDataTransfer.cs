using UnityEngine;

public class FightDataTransfer : MonoBehaviour
{
    public static FightDataTransfer Instance;
    
    public FightData fightData;

    public void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    public void SetData(FightData data)
    {
        fightData = data;
    }
}
