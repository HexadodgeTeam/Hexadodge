using UnityEngine;
using UnityEngine.Tilemaps;

public class FillGrid : MonoBehaviour
{
    public static FillGrid Instance { get; private set; }
    public static float ScaleX;
    public static float ScaleY;
    
    [SerializeField] private Rect gridBounds;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase filledTile;
    [SerializeField] private TileBase emptyTile;

    private int _filledTileCount;
    private int _totalTileCount;
    private Color _fillColor;

    private int FilledTileCount
    {
        get => _filledTileCount;
        set
        {
            _filledTileCount = value;
            UIManager.FillUI.SetFraction((float)_filledTileCount / _totalTileCount);

            if (_filledTileCount == _totalTileCount)
            {
                UIManager.SetAttackButton(true);
            }
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

    public void Draw(Vector3Int gridPos)
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

    public bool TryEmptyGrid()
    {
        if (FilledTileCount < _totalTileCount) return false;
        
        FilledTileCount = 0;
        tilemap.FloodFill(Vector3Int.zero, emptyTile);
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
