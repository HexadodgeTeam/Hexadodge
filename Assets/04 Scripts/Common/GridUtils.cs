using System;
using UnityEngine;

public static class GridUtils
{
    public static void ForCircle(this Action<Vector3Int> action,
        Func<Vector3, Vector3Int> worldToCell,
        Func<Vector3Int, Vector3> cellToWorld,
        Vector3 center, float radius)
    {
        Vector3Int centerCell = worldToCell.Invoke(center);
        int radiusCeil = Mathf.CeilToInt(radius);
        float radiusScaled = radius * FillGrid.ScaleX;

        for (int x = centerCell.x - radiusCeil; x <= centerCell.x + radiusCeil; x++)
        {
            for (int y = centerCell.y - radiusCeil; y <= centerCell.y + radiusCeil; y++)
            {
                Vector3Int cell = new Vector3Int(x, y);
                Vector3 worldPos = cellToWorld.Invoke(cell);

                if ((worldPos.x - center.x) * (worldPos.x - center.x) +
                    (worldPos.y - center.y) * (worldPos.y - center.y) <= radiusScaled * radiusScaled)
                {
                    action.Invoke(cell);
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

    private static Vector3Int[] GetNeighbourOffsets(Vector3Int cell)
    {
        return cell.y % 2 == 0 ? evenRowNeighbourOffsets : oddRowNeighbourOffsets;
    }

    public static int SumNeighbours(this Vector3Int cell, Func<Vector3Int, int> countFunc, int maxValue = int.MaxValue)
    {
        int sum = 0;
        
        foreach (Vector3Int offset in GetNeighbourOffsets(cell))
        {
            sum += countFunc.Invoke(cell + offset);

            if (sum >= maxValue) return sum; // Optimization to return early if sum is already enough.
        }

        return sum;
    }

    public static void ForEachNeighbour(this Action<Vector3Int> action, Vector3Int cell)
    {
        foreach (Vector3Int offset in GetNeighbourOffsets(cell))
        {
            action.Invoke(cell + offset);
        }
    }

    public static int CountNeighboursThat(this Vector3Int cell, Func<Vector3Int, bool> checkFunc)
    {
        int count = 0;
        
        foreach (Vector3Int offset in GetNeighbourOffsets(cell))
        {
            if (checkFunc.Invoke(cell + offset)) count++;
        }

        return count;
    }
}