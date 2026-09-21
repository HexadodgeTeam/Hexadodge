using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Color color = Color.white;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private int fillRange = 4;

    private Action<Vector3Int> _drawAction;

    private Vector3Int GridPos => transform.position.ToGrid();

    private void Start()
    {
        renderer.color = color;
        _drawAction = FillGrid.Instance.Draw;
        FillGrid.Instance.SetColor(color);
        UIManager.FillUI.SetColor(color);
    }

    public void TryAttack()
    {
        // The enemy damage is currently done in FillGrid. Should replace this with a skill/attack/fill level object.
        FillGrid.Instance.TryEmptyGrid(GridPos);
    }

    private void FixedUpdate()
    {
        _drawAction.ForCircle(GridPos, fillRange);
    }
}