using System;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    public static Vector2 Pos;
    public static Vector2 Delta;

    [SerializeField] private PlayerData data;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private PlayerHealth health;

    private Action<Vector3Int> _paint;

    public float FillRangeMultiplier { get; set; } = 1f;

    private void Start()
    {
        InitPaintAction();
        GetComponent<PlayerMovement>().Init(data.movementData);
        GetComponentInChildren<PlayerHealth>().Init(data.healthData);
    }

    public void InitPaintAction()
    {
        _paint = FillGrid.Instance.Paint;
    }

    private void FixedUpdate()
    {
        Delta = (Vector2)transform.position - Pos;
        Pos = transform.position;
        PaintCircle(data.fillRange * FillRangeMultiplier);
    }

    public void UpdateColor(Color color)
    {
        renderer.color = color;
        health.SetColor(color);
    }

    public void ChangeControlType(int value)
    {
        inputHandler.ChangeControlType(value);
    }
    
    public void PaintCircle(float radius)
    {
        if (!FillGrid.Instance.Active) return;
        
        _paint.Circle(transform.position, radius);
    }

    public void Teleport(Vector2 position)
    {
        inputHandler.Teleport(position);
        Pos = position;
    }
}