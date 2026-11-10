using TMPro;
using UnityEngine;

public class TextCountUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countText;
    [SerializeField] private string fieldName;

    public void SetCount(int count, bool hideIfZero = false)
    {
        countText.text = hideIfZero && count == 0 ? "" : $"{fieldName}{count}";
    }
}