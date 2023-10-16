using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexCoord
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    //public float offsetHeight;
    public int type;

    public HexCoord(int x, int y)
    {
        X = x;
        Y = y;
        Z = -x - y;
        //offsetHeight = 0;
        type = 0;
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
