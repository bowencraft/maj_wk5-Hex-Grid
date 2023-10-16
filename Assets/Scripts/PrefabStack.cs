using UnityEngine;
using System.Collections.Generic;

public class PrefabStack : MonoBehaviour
{
    public List<GameObject> prefabs; // 用于堆栈的prefab列表
    //public int AmountToSpawn;
    public Transform stackParent; // 用于存放堆栈的父对象
    public float offset = 0.5f; // 每个prefab之间的偏移量

    private List<GameObject> stack = new List<GameObject>();

    public void Start()
    {
    }

    // 初始化堆栈

    public void InitializeStack(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Count)]; // 随机选择一个Prefab
            GameObject go = Instantiate(randomPrefab, stackParent);
            SetLayer(go, 5);

            go.transform.localPosition = new Vector3(0, stack.Count * offset, 0);
            stack.Add(go);
        }
    }


    private void SetLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }

// 弹出堆栈的顶部prefab
    public GameObject Pop()
    {
        if (stack.Count > 0)
        {
            GameObject top = stack[stack.Count - 1];
            Destroy(stack[stack.Count - 1]);
            stack.RemoveAt(stack.Count - 1);
            return top;
        }
        return null;
    }

    public void RotateTop()
    {
        if (stack.Count > 0)
        {
            GameObject top = stack[stack.Count - 1];
            Transform objTransform = top.transform;
            objTransform.rotation *= Quaternion.Euler(0, 60, 0);

        }
    }

    public Quaternion GetRotation()
    {
        if (stack.Count > 0)
        {
            GameObject top = stack[stack.Count - 1];

            return top.transform.rotation;
        }
        return Quaternion.Euler(0, 0, 0);
    }
}
