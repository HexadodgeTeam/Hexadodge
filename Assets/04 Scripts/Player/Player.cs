using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Color color = Color.white;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private int fillRange = 4;

    private Action<Vector3Int> _drawAction;

    private void Start()
    {
        renderer.color = color;
        _drawAction = FillGrid.Instance.Draw;
        FillGrid.Instance.SetColor(color);
        UIManager.FillUI.SetColor(color);
    }

    public void Attack()
    {
        FillGrid.Instance.TryEmptyGrid();
    }

    private void FixedUpdate()
    {
        _drawAction.ForCircle(transform.position.ToGrid(), fillRange);
    }
}