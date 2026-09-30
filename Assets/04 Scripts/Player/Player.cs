using System;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Color fillColor1 = Color.white;
    [SerializeField] private Color fillColor2 = Color.white;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private float fillRange = 4;

    private Action<Vector3Int> _paint;
    public Vector3Int GridPos => FillGrid.Instance.WorldToCell(transform.position);

    private void Start()
    {
        _paint = FillGrid.Instance.Paint;
        renderer.color = color;
        FillGrid.Instance.SetColor(fillColor1, fillColor2);
        UIManager.FillUI.SetColor(color);
    }

    private void FixedUpdate()
    {
        if (FillGrid.Active)
        {
            _paint.Circle(transform.position, fillRange);
        }
    }
}