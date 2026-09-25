using System;
using UnityEngine;

public static class GridUtils
{
    public static void ForCircleOutline(this Action<Vector3Int> action, Vector3Int center, int radius)
    {
        int x = radius;
        int y = 0;
        int p = 1 - radius;

        while (x >= y)
        {
            action.Invoke(new Vector3Int(center.x + x + radius, center.y + y + radius));
            action.Invoke(new Vector3Int(center.x + y + radius, center.y + x + radius));
            action.Invoke(new Vector3Int(center.x + x + radius, center.y - y + radius));
            action.Invoke(new Vector3Int(center.x + y + radius, center.y - x + radius));
            action.Invoke(new Vector3Int(center.x - x + radius, center.y + y + radius));
            action.Invoke(new Vector3Int(center.x - y + radius, center.y + x + radius));
            action.Invoke(new Vector3Int(center.x - x + radius, center.y - y + radius));
            action.Invoke(new Vector3Int(center.x - y + radius, center.y - x + radius));

            if (p > 0) x--;
            y++;

            p += p > 0
                ? 1 - 2 * x + 2 * y
                : 1 + 2 * y;
        }
    }

    public static void ForCircle(this Action<Vector3Int> action, Vector3Int center, int radius)
    {
        for (int x = center.x - radius; x <= center.x; x++)
        {
            for (int y = center.y - radius; y <= center.y; y++)
            {
                if ((x - center.x) * (x - center.x) + (y - center.y) * (y - center.y) <= radius * radius)
                {
                    int xSym = 2 * center.x - x;
                    int ySym = 2 * center.y - y;

                    action.Invoke(new Vector3Int(x, y));
                    action.Invoke(new Vector3Int(xSym, y));
                    action.Invoke(new Vector3Int(x, ySym));
                    action.Invoke(new Vector3Int(xSym, ySym));
                }
            }
        }
    }

    public static void ForCircleFloat(this Action<Vector3Int> action,
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
}