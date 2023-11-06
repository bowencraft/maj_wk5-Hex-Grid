using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCell : MonoBehaviour
{
    public enum EdgeType
    {
        Plain,
        Road,
        Building,
        Forest,
        River
    }

    public HexCoord hexCoord;
    public EdgeType[] edgeTypes = new EdgeType[6]; // 每个六边形有6条边
    public GameObject[] edgeObjects = new GameObject[6];
    public GameObject models;

    // 当Prefab旋转时，旋转它的边的类型
    public void RotateClockwise()
    {
        EdgeType temp = edgeTypes[5];
        //GameObject tempObj = edgeObjects[5];
        for (int i = 5; i > 0; i--)
        {
            edgeTypes[i] = edgeTypes[i - 1];
            //edgeObjects[i] = edgeObjects[i - 1];
        }
        edgeTypes[0] = temp;
        //edgeObjects[0] = tempObj;
    }

    public EdgeType GetTypeForEdge(int edge)
    {
        return edgeTypes[edge];
    }

    public static bool CanMatch(EdgeType a, EdgeType b)
    {
        // 这只是一个简单的例子，你可以根据需要定义你自己的匹配规则
        if (a == EdgeType.Plain)
        {
            if (b == EdgeType.Plain || b == EdgeType.Forest || b == EdgeType.Building) return true;
        } else if (a == EdgeType.Forest)
        {
            if (b == EdgeType.Plain || b == EdgeType.Forest) return true;
        }
        else if (a == EdgeType.Building)
        {
            if (b == EdgeType.Plain || b == EdgeType.Building) return true;
        }
        else
        {
            return a == b;
        }
        return false;

    }

    public static int GetOppositeEdgeIndex(int edgeIndex)
    {
        return (edgeIndex + 3) % 6;
    }


    public void HideAllEdges()
    {
        foreach (GameObject item in edgeObjects)
        {
            item.SetActive(false);
        }
    }

    public void ShowEdge(int edge)
    {
        edgeObjects[edge].SetActive(true);
    }


}
