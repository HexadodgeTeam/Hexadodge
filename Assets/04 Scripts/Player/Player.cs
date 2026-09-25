using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private bool improvedDrawing = true;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Color fillColor1 = Color.white;
    [SerializeField] private Color fillColor2 = Color.white;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private float fillRange = 4;

    private Action<Vector3Int> _drawAction;

    private Vector3Int GridPos => FillGrid.Instance.WorldToGrid(transform.position);

    private void Start()
    {
        renderer.color = color;
        _drawAction = FillGrid.Instance.Draw;
        FillGrid.Instance.SetColor(fillColor1, fillColor2);
        UIManager.FillUI.SetColor(color);
    }

    public void TryAttack()
    {
        // The enemy damage is currently done in FillGrid. Should replace this with a skill/attack/fill level object.
        FillGrid.Instance.TryEmptyGrid(GridPos);
    }

    private void FixedUpdate()
    {
        if (improvedDrawing)
        {
            _drawAction.ForCircleFloat(FillGrid.Instance.WorldToGrid, FillGrid.Instance.GridToWorld, transform.position, fillRange);
        }
        else
        {
            _drawAction.ForCircle(GridPos, (int)fillRange);
        }
    }
}