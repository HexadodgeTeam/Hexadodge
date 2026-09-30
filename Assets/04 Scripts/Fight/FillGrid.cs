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
    private static float ScaleY;
    public static bool Active { get; private set; }

    [Header("Bounds")]
    [SerializeField] private Rect gridBounds;

    [Header("Grow")]
    [SerializeField] private float minFilledCellScale = 1f;
    [SerializeField] private float maxFilledCellScale = 1.5f;
    [SerializeField] private float filledCellGrowDuration = 1f;
    [SerializeField] private AnimationCurve growCurve;

    [Header("Glow")]
    [SerializeField] [Range(0f, 1f)] private float glowGammaGain = 0.1f;
    [SerializeField] private float filledCellGlowDuration = 1f;
    [SerializeField] private AnimationCurve glowCurve;

    [Header("Capillarity")]
    [SerializeField] private int capillarityTriggerLevel = 5;
    [SerializeField] private int edgeCapillarityLevel = 2;
    [SerializeField] private int paintCapillarityLevel = 1;
    [SerializeField] private float capillaritySpreadTime = 0.2f;

    [Header("Empty animation")]
    [SerializeField] private float emptyAnimationDuration = 1f;
    [SerializeField] private AnimationCurve emptyAnimationCurve;
    [SerializeField] private AnimationCurve tileShrinkAnimationCurve;
    [SerializeField] private bool tileShrink = true;
    [SerializeField] private Vector3 tileShrinkTarget = new Vector3(0.7f, 0.7f, 0.7f);

    [Header("References")]
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] private Tilemap paintTilemap;
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
        Active = true;
        Instance = this;
        ScaleX = transform.localScale.x;
        ScaleY = transform.localScale.y;

        _totalTileCount = backgroundTilemap.GetTilesRangeCount(
            new Vector3Int((int)(gridBounds.xMin / ScaleX), (int)(gridBounds.yMin / ScaleY)),
            new Vector3Int((int)(gridBounds.xMax / ScaleX), (int)(gridBounds.yMax / ScaleY)));
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return paintTilemap.GetCellCenterWorld(cell);
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return paintTilemap.WorldToCell(worldPos);
    }

    public void Paint(Vector3Int cell)
    {
        if (!Active || !IsInGrid(cell)) return;

        if (!IsFilled(cell)) FillCell(cell);
        GlowCell(cell);

        if (!_capillarityRoutines.ContainsKey(cell))
        {
            _capillarityRoutines[cell] = StartCoroutine(CellCapillarityRoutine(cell));
        }
    }

    private IEnumerator CellCapillarityRoutine(Vector3Int cell)
    {
        yield return new WaitForSeconds(capillaritySpreadTime);

        ((Action<Vector3Int>)CapillaritySpread).ForEachNeighbour(cell);
        _capillarityRoutines.Remove(cell);
    }

    private void CapillaritySpread(Vector3Int cell)
    {
        if (!Active || !IsInGrid(cell) || IsFilled(cell)) return;

        if (cell.SumNeighbours(CapillarityLevel, capillarityTriggerLevel) >= capillarityTriggerLevel)
        {
            Paint(cell);
        }
    }

    /// <summary>
    /// Set a cell to "filled" state and animate it, without checking for empty zones.
    /// </summary>
    /// <param name="cell">Cell position.</param>
    private void FillCell(Vector3Int cell)
    {
        float randomScale = Random.Range(minFilledCellScale, maxFilledCellScale);
        paintTilemap.SetTile(cell, RandomFilledTile());
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
            paintTilemap.SetTransformMatrix(cell, Matrix4x4.Scale(Vector3.Lerp(Vector3.one, targetScale, p)));
            yield return null;
        }

        paintTilemap.SetTransformMatrix(cell, Matrix4x4.Scale(targetScale));
    }

    private IEnumerator GlowRoutine(Vector3Int cell, Color color)
    {
        Color glowColor = new Color(color.r + glowGammaGain, color.g + glowGammaGain, color.b + glowGammaGain);

        float t = 0f;
        while (t < filledCellGlowDuration)
        {
            t += Time.deltaTime;
            float p = glowCurve.Evaluate(t / filledCellGlowDuration);
            paintTilemap.SetColor(cell, Color.Lerp(glowColor, color, p));
            yield return null;
        }

        _activeGlows.Remove(cell);
    }

    private bool IsInGrid(Vector3Int cell)
    {
        return backgroundTilemap.HasTile(cell);
    }

    private bool IsFilled(Vector3Int cell)
    {
        return paintTilemap.HasTile(cell);
    }

    public void SetColor(Color color1, Color color2)
    {
        _fillColor1 = color1;
        _fillColor2 = color2;
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
        float lower = backgroundTilemap.cellBounds.yMin;
        float upper = backgroundTilemap.cellBounds.yMax;
        return Color.Lerp(_fillColor1, _fillColor2, Mathf.InverseLerp(lower, upper, cell.y));
    }

    private int CapillarityLevel(Vector3Int cell)
    {
        if (IsFilled(cell)) return paintCapillarityLevel;
        if (!IsInGrid(cell)) return edgeCapillarityLevel;
        return 0;
    }

    #region Emptying

    public void EmptyAt(Vector3Int cell)
    {
        _activeGlows.Clear();
        _capillarityRoutines.Clear();
        FilledTileCount = 0;
        Active = false;
        StartCoroutine(EmptyAnimation());
    }

    private IEnumerator EmptyAnimation()
    {
        float shrinkTime = emptyAnimationDuration / gridBounds.size.y;

        void EmptyRow(int y)
        {
            for (int x = backgroundTilemap.cellBounds.xMin; x <= backgroundTilemap.cellBounds.xMax; x++)
            {
                StartCoroutine(ShrinkRoutine(new Vector3Int(x, y), shrinkTime));
            }
        }

        int emptyingRow = backgroundTilemap.cellBounds.yMax;

        float t = 0f;
        while (t < emptyAnimationDuration)
        {
            t += Time.deltaTime;

            int nextRow = (int)Mathf.Lerp(
                backgroundTilemap.cellBounds.yMax,
                backgroundTilemap.cellBounds.yMin,
                emptyAnimationCurve.Evaluate(t / emptyAnimationDuration));

            if (nextRow != emptyingRow)
            {
                for (int r = emptyingRow; r >= nextRow; r--)
                {
                    EmptyRow(r);
                }

                emptyingRow = nextRow;
            }

            yield return null;
        }

        Active = true;
    }

    private IEnumerator ShrinkRoutine(Vector3Int cell, float shrinkTime)
    {
        if (tileShrink)
        {
            Matrix4x4 m = paintTilemap.GetTransformMatrix(cell);
            Vector3 baseScale = new Vector3(
                m.GetColumn(0).magnitude,
                m.GetColumn(1).magnitude,
                m.GetColumn(2).magnitude
            );
            float t = 0f;
            while (t < shrinkTime)
            {
                t += Time.deltaTime;
                float p = tileShrinkAnimationCurve.Evaluate(t / shrinkTime);
                paintTilemap.SetTransformMatrix(cell,
                    Matrix4x4.Scale(Vector3.Lerp(baseScale, tileShrinkTarget, p)));
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(shrinkTime);
        }

        paintTilemap.SetTile(cell, null);
    }

    #endregion
}