using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FillGrid : MonoBehaviour
{
    public static FillGrid Instance { get; private set; }
    public static float ScaleX;
    public static float ScaleY;

    [SerializeField] private float minFilledCellScale = 1f;
    [SerializeField] private float maxFilledCellScale = 1.5f;
    [SerializeField] private float filledCellGrowDuration = 1f;
    [SerializeField] private AnimationCurve growCurve;
    [SerializeField] private float filledCellGlowDuration = 1f;
    [SerializeField] private AnimationCurve glowCurve;
    [SerializeField] private Rect gridBounds;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap emptyTilemap;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] filledTiles;
    [SerializeField] private TileBase emptyTile;
    [SerializeField] private float[] skillFractions;
    
    // TODO TEMP REFERENCE
    [SerializeField] private Enemy enemy;

    private int _filledTileCount;
    private int _totalTileCount;
    private Color _fillColor1;
    private Color _fillColor2;
    private int _skillNumber;

    private readonly Dictionary<Vector3Int, Coroutine> _activeGlows = new();

    private int FilledTileCount
    {
        get => _filledTileCount;
        set
        {
            _filledTileCount = value;

            float fraction = _filledTileCount / (float)_totalTileCount;
            
            UIManager.FillUI.SetFraction(fraction);

            _skillNumber = 0;

            foreach (var skillFraction in skillFractions)
            {
                if (fraction >= skillFraction)
                {
                    _skillNumber++;
                }
            }

            UIManager.SkillNumber.SetCount(_skillNumber, true);
            UIManager.SetAttackButton(_skillNumber > 0);
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

    private void Start()
    {
        UIManager.SetThresholdIndicators(skillFractions);
    }

    public Vector3 GridToWorld(Vector3Int gridPos)
    {
        return tilemap.GetCellCenterWorld(gridPos);
    }

    public Vector3Int WorldToGrid(Vector3 worldPos)
    {
        return tilemap.WorldToCell(worldPos);
    }
    
    public void Draw(Vector3Int gridPos)
    {
        if (!IsInGrid(gridPos)) return;

        if (!tilemap.HasTile(gridPos))
        {
            float randomScale = Random.Range(minFilledCellScale, maxFilledCellScale);
            tilemap.SetTile(gridPos, RandomFilledTile());
            StartCoroutine(GrowRoutine(gridPos, new Vector3(randomScale, randomScale, 1f)));
 
            FilledTileCount++;
        }

        if (_activeGlows.TryGetValue(gridPos, out Coroutine activeGlow))
        {
            StopCoroutine(activeGlow);
        }
        _activeGlows[gridPos] = StartCoroutine(GlowRoutine(gridPos, GetTileColor(gridPos)));
    }
 
    private IEnumerator GrowRoutine(Vector3Int gridPos, Vector3 targetScale)
    {
        float t = 0f;
        
        while (t < filledCellGrowDuration)
        {
            t += Time.deltaTime;
            float p = growCurve.Evaluate(t / filledCellGrowDuration);
            tilemap.SetTransformMatrix(gridPos, Matrix4x4.Scale(Vector3.Lerp(Vector3.one, targetScale, p)));
            yield return null;
        }
 
        tilemap.SetTransformMatrix(gridPos, Matrix4x4.Scale(targetScale));
    }

    private IEnumerator GlowRoutine(Vector3Int gridPos, Color color)
    {
        float t = 0f;
        Color glowColor = new Color(color.r + 0.07f, color.g + 0.07f, color.b + 0.07f);

        while (t < filledCellGlowDuration)
        {
            t += Time.deltaTime;
            float p = glowCurve.Evaluate(t / filledCellGlowDuration);
            tilemap.SetColor(gridPos, Color.Lerp(glowColor, color, p));
            yield return null;
        }
        
        _activeGlows.Remove(gridPos);
    }


    private bool IsInGrid(Vector3Int gridPos)
    {
        return emptyTilemap.HasTile(gridPos);
    }

    public void SetColor(Color color1, Color color2)
    {
        _fillColor1 = color1;
        _fillColor2 = color2;
    }

    public bool TryEmptyGrid(Vector3Int gridPos)
    {
        if (_skillNumber <= 0) return false;
        
        enemy.TakeDamage(_skillNumber);
        FilledTileCount = 0;
        tilemap.FloodFill(gridPos, null);
        UIManager.SetAttackButton(false);
        
        return true;
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

    private Color GetTileColor(Vector3Int gridPos)
    {
        float lower = emptyTilemap.cellBounds.yMin;
        float upper = emptyTilemap.cellBounds.yMax;
        return Color.Lerp(_fillColor1, _fillColor2, Mathf.InverseLerp(lower, upper, gridPos.y));;
    }
}
