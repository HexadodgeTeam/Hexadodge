using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsPanel : UIPanel
{
    [SerializeField] private TMP_Text sensitivityText;
    [SerializeField] private Slider sensitivitySlider;

    private void Awake()
    {
        sensitivitySlider.value = PlayerPrefs.HasKey("Sensitivity") ? PlayerPrefs.GetFloat("Sensitivity") : 1.5f;
    }

    public void UpdateSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        sensitivityText.text = "Sensitivity: " + value.ToString("0.00");
        PlayerInputHandler.PlayerSensitivity = value;
    }
}