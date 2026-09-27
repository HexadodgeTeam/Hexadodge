using System;
using UnityEngine;

public static class GridUtils
{
    public static void ForCircle(this Action<Vector3Int> action,
        Func<Vector3, Vector3Int> worldToCell,
        Func<Vector3Int, Vector3> cellToWorld,
        Vector3 center, float radius)
    {
        Vector3Int centerGridPos = worldToCell.Invoke(center);
        int radiusCeil = Mathf.CeilToInt(radius);
        float radiusScaled = radius * FillGrid.ScaleX;

        for (int x = centerGridPos.x - radiusCeil; x <= centerGridPos.x + radiusCeil; x++)
        {
            for (int y = centerGridPos.y - radiusCeil; y <= centerGridPos.y + radiusCeil; y++)
            {
                Vector3Int gridPos = new Vector3Int(x, y);
                Vector3 worldPos = cellToWorld.Invoke(gridPos);

                if ((worldPos.x - center.x) * (worldPos.x - center.x) +
                    (worldPos.y - center.y) * (worldPos.y - center.y) <= radiusScaled * radiusScaled)
                {
                    action.Invoke(gridPos);
                }
            }
        }
    }

    private static readonly Vector3Int[] evenRowNeighbourOffsets = {
         new(-1, 0),
         new(1, 0),
         new(0, -1),
         new(0, 1),
         new(-1, -1),
         new(-1, 1)
    };
    
    private static readonly Vector3Int[] oddRowNeighbourOffsets = {
        new(-1, 0),
        new(1, 0),
        new(0, -1),
        new(0, 1),
        new(1, -1),
        new(1, 1)
    };

    private static Vector3Int[] GetNeighbourOffsets(Vector3Int gridPos)
    {
        return gridPos.y % 2 == 0 ? evenRowNeighbourOffsets : oddRowNeighbourOffsets;
    }
    
    public static int CountForNeighbours(this Func<Vector3Int, int> countFunc, Vector3Int gridPos)
    {
        int count = 0;
        
        foreach (Vector3Int offset in GetNeighbourOffsets(gridPos))
        {
            count += countFunc.Invoke(gridPos + offset);
        }

        return count;
    }

    public static void ForNeighboursIf(this Action action, Func<Vector3Int, bool> checkFunc, Vector3Int gridPos, Action cleanAction)
    {
        bool isOnEvenRow = gridPos.y % 2 == 0;
        
        foreach (Vector3Int offset in isOnEvenRow ? evenRowNeighbourOffsets : oddRowNeighbourOffsets)
        {
            if (checkFunc.Invoke(gridPos + offset))
            {
                action.Invoke();
            }
            
            cleanAction.Invoke();
        }
    }
}