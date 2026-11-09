using System;
using System.Collections.Generic;
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
        _paint = FillGrid.Instance.Paint;
        GetComponent<PlayerMovement>().Init(data.movementData);
        GetComponentInChildren<PlayerHealth>().Init(data.healthData);
    }

    private void FixedUpdate()
    {
        Delta = (Vector2)transform.position - Pos;
        Pos = transform.position;
        if (FillGrid.Active)
        {
            _paint.Circle(transform.position, data.fillRange * FillRangeMultiplier);
        }
    }

    public void UpdateColor(Color color)
    {
        renderer.color = color;
    }

    public void ChangeControlType(int value)
    {
        inputHandler.ChangeControlType(value);
    }
}