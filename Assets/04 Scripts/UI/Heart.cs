using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField] private Image inside;
    
    public void Full(Color color)
    {
        inside.enabled = true;
        inside.color = color;
    }

    public void Empty()
    {
        inside.enabled = false;
    }
}
