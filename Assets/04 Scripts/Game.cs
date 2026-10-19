using UnityEngine;

public class Game : MonoBehaviour
{
    public static float TimeScale { private get; set; } = 1f;
    public static float DeltaTime => Time.deltaTime * TimeScale;
    public static float ScaledTime { get; private set; } = 0f;
    
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
    }

    private void FixedUpdate()
    {
        ScaledTime += DeltaTime;
    }

    public static void SetColor(Color color, Gradient gradient)
    {
        Player.UpdateColor(color);
        FillGrid.Instance.SetGradient(gradient);
        UIManager.Instance.fillUI.SetColor(color);
    }
}
