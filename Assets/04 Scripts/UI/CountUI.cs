using TMPro;
using UnityEngine;

public class CountUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countText;
    [SerializeField] private string fieldName;

    public void SetCount(int count)
    {
        countText.text = $"{fieldName}: {count}";
    }
}
