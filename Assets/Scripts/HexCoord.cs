using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public struct HexCoord
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    //public float offsetHeight;
    public PrefabCell prefabCell;

    public HexCoord(int x, int y)
    {
        X = x;
        Y = y;
        Z = -x - y;
        //offsetHeight = 0;
        prefabCell = null;
    }


    public override bool Equals(object obj)
    {
        if (obj is HexCoord thisCoord)
        {
            return (X == thisCoord.X && Y == thisCoord.Y);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (X.ToString()+","+Y.ToString()).GetHashCode();
    }

    public Vector3 GetEdgeMidpointWorldPos(int edgeIndex, float size)
    {
        // 六边形边的方向
        Vector2[] directions =
        {
        new Vector2(0, 1),
        new Vector2(0.75f, 0.5f),
        new Vector2(0.75f, -0.5f),
        new Vector2(0, -1),
        new Vector2(-0.75f, -0.5f),
        new Vector2(-0.75f, 0.5f)
    };

        Vector2 dir = directions[edgeIndex];
        Vector3 worldPos = HexCoord.ToWorldPos(this, size);

        return new Vector3(worldPos.x + dir.x * size, worldPos.y, worldPos.z + dir.y * size);
    }

    public float GetEdgeAngle(int edgeIndex)
    {
        float[] angles =
        {
        90,   // 上
        60,   // 右上
        300,  // 右下
        270,  // 下
        240,  // 左下
        120   // 左上
    };

        return angles[edgeIndex];
    }

    public int? GetNeighborCellEdgeIndex(HexCoord other)
    {
        int dx = other.X - this.X;
        int dy = other.Y - this.Y;

        if (dx == 0 && dy == 1)
            return 0;
        else if (dx == 1 && dy == 0)
            return 5;
        else if (dx == 1 && dy == -1)
            return 4;
        else if (dx == 0 && dy == -1)
            return 3;
        else if (dx == -1 && dy == 0)
            return 2;
        else if (dx == -1 && dy == 1)
            return 1;
        else
            return null;  // 不是相邻的cell
    }


    public static HexCoord FromOffset(int col, int row)
    {
        int x = col - (row + (row & 1)) / 2;
        int y = row;
        return new HexCoord(x, y);
    }

    public static Vector3 ToWorldPos(HexCoord hex, float size)
    {
        float x = size * 1.5f * hex.X;
        float z = -size * Mathf.Sqrt(3) * (hex.Y + hex.X / 2f); // 注意这里的负号
        return new Vector3(x, 0, z);
    }

}
