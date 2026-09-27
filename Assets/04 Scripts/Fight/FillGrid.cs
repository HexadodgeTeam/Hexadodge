using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class FillGrid : MonoBehaviour
{
    public static FillGrid Instance { get; private set; }
    public static float ScaleX;
    public static float ScaleY;

    [Header("Parameters")]
    [SerializeField] private Rect gridBounds;
    [SerializeField] private float minFilledCellScale = 1f;
    [SerializeField] private float maxFilledCellScale = 1.5f;
    [SerializeField] private float filledCellGrowDuration = 1f;
    [SerializeField] private AnimationCurve growCurve;
    [SerializeField] private float filledCellGlowDuration = 1f;
    [SerializeField] private AnimationCurve glowCurve;
    [SerializeField] private int maxEmptyCellAutoFill = 5;
    
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
    private readonly HashSet<Vector3Int> _checkedEmptyTiles = new();
    
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

    public Vector3 CellToWorld(Vector3Int cellPos)
    {
        return tilemap.GetCellCenterWorld(cellPos);
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return tilemap.WorldToCell(worldPos);
    }
    
    public void Draw(Vector3Int cellPos)
    {
        if (!IsInGrid(cellPos)) return;

        FillCell(cellPos);
        
        ((Action)FillCheckedEmptyTiles).ForNeighboursIf(ShouldEmptyZoneAutoFill, cellPos, ClearCheckedEmptyTiles);
    }

    private bool ShouldEmptyZoneAutoFill(Vector3Int cellPos) => CountEmptyZoneSize(cellPos) <= maxEmptyCellAutoFill;
    
    /// <summary>
    /// Recursively count an empty zone size.
    /// </summary>
    /// <param name="cellPos">Start position from which to check.</param>
    /// <returns>Empty zone size.</returns>
    private int CountEmptyZoneSize(Vector3Int cellPos)
    {
        if (tilemap.HasTile(cellPos)) return 0;
        if (!_checkedEmptyTiles.Add(cellPos)) return 0;
        if (_checkedEmptyTiles.Count > maxEmptyCellAutoFill) return maxEmptyCellAutoFill + 1;

        int zoneSize = ((Func<Vector3Int, int>)CountEmptyZoneSize).CountForNeighbours(cellPos) + 1;
        
        return zoneSize > maxEmptyCellAutoFill ? maxEmptyCellAutoFill + 1 : zoneSize;
    }

    /// <summary>
    /// Set a cell to "filled" state and animate it, without checking for empty zones.
    /// </summary>
    /// <param name="cellPos">Cell position.</param>
    private void FillCell(Vector3Int cellPos)
    {
        if (!tilemap.HasTile(cellPos))
        {
            float randomScale = Random.Range(minFilledCellScale, maxFilledCellScale);
            tilemap.SetTile(cellPos, RandomFilledTile());
            StartCoroutine(GrowRoutine(cellPos, new Vector3(randomScale, randomScale, 1f)));
 
            FilledTileCount++;
        }

        if (_activeGlows.TryGetValue(cellPos, out Coroutine activeGlow))
        {
            StopCoroutine(activeGlow);
        }
        _activeGlows[cellPos] = StartCoroutine(GlowRoutine(cellPos, GetTileColor(cellPos)));
    }
 
    private IEnumerator GrowRoutine(Vector3Int cellPos, Vector3 targetScale)
    {
        float t = 0f;
        
        while (t < filledCellGrowDuration)
        {
            t += Time.deltaTime;
            float p = growCurve.Evaluate(t / filledCellGrowDuration);
            tilemap.SetTransformMatrix(cellPos, Matrix4x4.Scale(Vector3.Lerp(Vector3.one, targetScale, p)));
            yield return null;
        }
 
        tilemap.SetTransformMatrix(cellPos, Matrix4x4.Scale(targetScale));
    }

    private IEnumerator GlowRoutine(Vector3Int cellPos, Color color)
    {
        float t = 0f;
        Color glowColor = new Color(color.r + 0.07f, color.g + 0.07f, color.b + 0.07f);

        while (t < filledCellGlowDuration)
        {
            t += Time.deltaTime;
            float p = glowCurve.Evaluate(t / filledCellGlowDuration);
            tilemap.SetColor(cellPos, Color.Lerp(glowColor, color, p));
            yield return null;
        }
        
        _activeGlows.Remove(cellPos);
    }


    private bool IsInGrid(Vector3Int cellPos)
    {
        return emptyTilemap.HasTile(cellPos);
    }

    public void SetColor(Color color1, Color color2)
    {
        _fillColor1 = color1;
        _fillColor2 = color2;
    }

    public void EmptyAt(Vector3Int cellPos)
    {
        FilledTileCount = 0;
        tilemap.FloodFill(cellPos, null);
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

    private Color GetTileColor(Vector3Int cellPos)
    {
        float lower = emptyTilemap.cellBounds.yMin;
        float upper = emptyTilemap.cellBounds.yMax;
        return Color.Lerp(_fillColor1, _fillColor2, Mathf.InverseLerp(lower, upper, cellPos.y));
    }

    private void ClearCheckedEmptyTiles() => _checkedEmptyTiles.Clear();
    private void FillCheckedEmptyTiles() => FillCells(_checkedEmptyTiles);
    
    /// <summary>
    /// Fills all cell in a collection.
    /// </summary>
    /// <param name="cells">The cell collection.</param>
    private void FillCells(ICollection<Vector3Int> cells)
    {
        foreach (Vector3Int tile in cells)
        {
            FillCell(tile);
        }
    }
}
