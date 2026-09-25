using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
    
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
