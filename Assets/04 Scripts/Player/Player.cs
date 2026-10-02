using System;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    public static Vector2 Pos;
    
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Gradient fillGradient;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private float fillRange = 4;

    private Action<Vector3Int> _paint;
    public Vector3Int GridPos => FillGrid.Instance.WorldToCell(transform.position);

    private void Start()
    {
        _paint = FillGrid.Instance.Paint;
        renderer.color = color;
        FillGrid.Instance.SetGradient(fillGradient);
        UIManager.FillUI.SetColor(color);
    }

    private void FixedUpdate()
    {
        Pos = transform.position;
        if (FillGrid.Active)
        {
            _paint.Circle(transform.position, fillRange);
        }
    }
}