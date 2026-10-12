using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
    }

    public static void SetColor(Color color, Gradient gradient)
    {
        Player.UpdateColor(color);
        FillGrid.Instance.SetGradient(gradient);
        UIManager.FillUI.SetColor(color);
    }
}
