using System;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    public static Vector2 Pos;
    public static Vector2 Delta;

    private static SpriteRenderer Renderer;
    
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private float fillRange = 4;

    private Action<Vector3Int> _paint;

    private void Awake()
    {
        Renderer = renderer;
    }

    private void Start()
    {
        _paint = FillGrid.Instance.Paint;
    }

    private void FixedUpdate()
    {
        Delta = (Vector2)transform.position - Pos;
        Pos = transform.position;
        if (FillGrid.Active)
        {
            _paint.Circle(transform.position, fillRange);
        }
    }

    public static void UpdateColor(Color color)
    {
        Renderer.color = color;
    }
}