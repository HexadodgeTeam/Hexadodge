using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class FillGrid : MonoBehaviour
{
    public static FillGrid Instance { get; private set; }
    public static float ScaleX;
    public static float ScaleY;

    [Header("Parameters")] [SerializeField]
    private Rect gridBounds;

    [SerializeField] private float minFilledCellScale = 1f;
    [SerializeField] private float maxFilledCellScale = 1.5f;
    [SerializeField] private float filledCellGrowDuration = 1f;
    [SerializeField] private AnimationCurve growCurve;
    [SerializeField] private float filledCellGlowDuration = 1f;
    [SerializeField] private AnimationCurve glowCurve;
    [SerializeField] private int capillarityTriggerLevel = 5;
    [SerializeField] private int edgeCapillarityLevel = 2;
    [SerializeField] private int paintCapillarityLevel = 1;
    [SerializeField] private float capillaritySpreadTime = 0.2f;

    [Header("References")]
    [SerializeField] private Grid grid;

    [SerializeField] private Tilemap emptyTilemap;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] filledTiles;
    [SerializeField] private TileBase emptyTile;

    private int _filledTileCount;
    private int _totalTileCount;
    private Color _fillColor1;
    private Color _fillColor2;
    private readonly Dictionary<Vector3Int, Coroutine> _activeGlows = new();
    private readonly Dictionary<Vector3Int, Coroutine> _capillarityRoutines = new();

    public Action<float> OnFillUpdated;

    private int FilledTileCount
    {
        get => _filledTileCount;
        set
        {
            _filledTileCount = value;
            OnFillUpdated.Invoke(_filledTileCount / (float)_totalTileCount);
        }
    }

    private void Awake()
    {
        Instance = this;
        ScaleX = transform.localScale.x;
        ScaleY = transform.localScale.y;

        _totalTileCount = emptyTilemap.GetTilesRangeCount(
            new Vector3Int((int)(gridBounds.xMin / ScaleX), (int)(gridBounds.yMin / ScaleY)),
            new Vector3Int((int)(gridBounds.xMax / ScaleX), (int)(gridBounds.yMax / ScaleY)));
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return tilemap.GetCellCenterWorld(cell);
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return tilemap.WorldToCell(worldPos);
    }

    public void Paint(Vector3Int cell)
    {
        if (!IsInGrid(cell)) return;
        
        if (!tilemap.HasTile(cell))
        {
            FillCell(cell);
        }
        
        GlowCell(cell);
        ((Action<Vector3Int>)CapillaritySpread).ForNeighbours(cell);
    }
    
    private void CapillaritySpread(Vector3Int cell)
    {
        if (!IsInGrid(cell) || tilemap.HasTile(cell) || _capillarityRoutines.ContainsKey(cell)) return;
        
        if (cell.SumNeighbours(CapillarityLevel) >= capillarityTriggerLevel)
        {
            _capillarityRoutines[cell] = StartCoroutine(CapillaritySpreadRoutine(cell));
        }
    }

    private IEnumerator CapillaritySpreadRoutine(Vector3Int cell)
    {
        yield return new WaitForSeconds(capillaritySpreadTime);
        
        Paint(cell);
        _capillarityRoutines.Remove(cell);
    }

    /// <summary>
    /// Set a cell to "filled" state and animate it, without checking for empty zones.
    /// </summary>
    /// <param name="cell">Cell position.</param>
    private void FillCell(Vector3Int cell)
    {
        float randomScale = Random.Range(minFilledCellScale, maxFilledCellScale);
        tilemap.SetTile(cell, RandomFilledTile());
        StartCoroutine(GrowRoutine(cell, new Vector3(randomScale, randomScale, 1f)));

        FilledTileCount++;
    }

    private void GlowCell(Vector3Int cell)
    {
        if (_activeGlows.TryGetValue(cell, out Coroutine activeGlow))
        {
            StopCoroutine(activeGlow);
        }

        _activeGlows[cell] = StartCoroutine(GlowRoutine(cell, GetTileColor(cell)));
    }

    private IEnumerator GrowRoutine(Vector3Int cell, Vector3 targetScale)
    {
        float t = 0f;

        while (t < filledCellGrowDuration)
        {
            t += Time.deltaTime;
            float p = growCurve.Evaluate(t / filledCellGrowDuration);
            tilemap.SetTransformMatrix(cell, Matrix4x4.Scale(Vector3.Lerp(Vector3.one, targetScale, p)));
            yield return null;
        }

        tilemap.SetTransformMatrix(cell, Matrix4x4.Scale(targetScale));
    }

    private IEnumerator GlowRoutine(Vector3Int cell, Color color)
    {
        float t = 0f;
        Color glowColor = new Color(color.r + 0.07f, color.g + 0.07f, color.b + 0.07f);

        while (t < filledCellGlowDuration)
        {
            t += Time.deltaTime;
            float p = glowCurve.Evaluate(t / filledCellGlowDuration);
            tilemap.SetColor(cell, Color.Lerp(glowColor, color, p));
            yield return null;
        }

        _activeGlows.Remove(cell);
    }


    private bool IsInGrid(Vector3Int cell)
    {
        return emptyTilemap.HasTile(cell);
    }

    public void SetColor(Color color1, Color color2)
    {
        _fillColor1 = color1;
        _fillColor2 = color2;
    }

    public void EmptyAt(Vector3Int cell)
    {
        FilledTileCount = 0;
        tilemap.FloodFill(cell, null);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3(gridBounds.center.x, gridBounds.center.y),
            new Vector3(gridBounds.size.x, gridBounds.size.y));
    }

    private TileBase RandomFilledTile()
    {
        return filledTiles[Random.Range(0, filledTiles.Length)];
    }

    private Color GetTileColor(Vector3Int cell)
    {
        float lower = emptyTilemap.cellBounds.yMin;
        float upper = emptyTilemap.cellBounds.yMax;
        return Color.Lerp(_fillColor1, _fillColor2, Mathf.InverseLerp(lower, upper, cell.y));
    }

    private int CapillarityLevel(Vector3Int cell)
    {
        if (tilemap.HasTile(cell)) return paintCapillarityLevel;
        if (!IsInGrid(cell)) return edgeCapillarityLevel;
        return 0;
    }
}
