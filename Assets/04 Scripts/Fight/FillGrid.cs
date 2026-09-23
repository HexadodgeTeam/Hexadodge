using UnityEngine;
using UnityEngine.Tilemaps;

public class FillGrid : MonoBehaviour
{
    public static FillGrid Instance { get; private set; }
    public static float ScaleX;
    public static float ScaleY;
    
    [SerializeField] private Rect gridBounds;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase filledTile;
    [SerializeField] private TileBase emptyTile;
    [SerializeField] private float[] skillFractions;
    
    // TODO TEMP REFERENCE
    [SerializeField] private Enemy enemy;

    private int _filledTileCount;
    private int _totalTileCount;
    private Color _fillColor;
    private int _skillNumber;

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

        _totalTileCount = tilemap.GetTilesRangeCount(
            new Vector3Int((int)(gridBounds.xMin / ScaleX), (int)(gridBounds.yMin / ScaleY)),
            new Vector3Int((int)(gridBounds.xMax / ScaleX), (int)(gridBounds.yMax / ScaleY)));
    }

    private void Start()
    {
        UIManager.SetThresholdIndicators(skillFractions);
    }

    public Vector3 GetCellWorldPos(Vector3Int gridPos)
    {
        return tilemap.CellToWorld(gridPos);
        return tilemap.cente
    }
    
    private void SetTile(Vector3Int gridPos)
    {
        if (!IsInGrid(gridPos) || tilemap.GetTile(gridPos) == filledTile) return;
        
        tilemap.SetTile(gridPos, filledTile);
        tilemap.SetColor(gridPos, _fillColor);
        FilledTileCount++;
    }

    private bool IsInGrid(Vector3Int gridPos)
    {
        return tilemap.HasTile(gridPos);
    }

    public void SetColor(Color color)
    {
        _fillColor = color;
    }

    public bool TryEmptyGrid(Vector3Int gridPos)
    {
        if (_skillNumber <= 0) return false;
        
        enemy.TakeDamage(_skillNumber);
        FilledTileCount = 0;
        tilemap.FloodFill(gridPos, emptyTile);
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
}
