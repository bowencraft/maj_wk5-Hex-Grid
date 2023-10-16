using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int Width = 10;
    public int Height = 10;
    public float HexSize = 1.0f;
    public GameObject HexPrefab;

    //public float prefabSize = 1.725f;
    public float prefabAngle = 90f;

    public List<HexCoord> ActiveCoords = new List<HexCoord>();
    public List<HexCoord> BackgroundCoords = new List<HexCoord>();
    //public List AllCoords

    private PrefabStack PrefabStack;

    public List<GameObject> prefabs;
    public int AmountToSpawn;

    private void Awake()
    {
        //GenerateGrid();
        HexCoord coord = HexCoord.FromOffset(0, 0);
        CreateBackgroundCoord(coord,0);

        PrefabStack = FindObjectOfType<PrefabStack>();
        PrefabStack.prefabs = prefabs;
        //PrefabStack.AmountToSpawn = AmountToSpawn;
        PrefabStack.InitializeStack(AmountToSpawn);

    }

    public void Start()
    {
    }

    void GenerateGrid()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                HexCoord coord = HexCoord.FromOffset(j, i);
                BackgroundCoords.Add(coord);
                Vector3 pos = HexCoord.ToWorldPos(coord, HexSize);
                //pos.y += Mathf.PerlinNoise(xCoord, yCoord);
                pos.y -= 0.4f;
                Instantiate(HexPrefab, pos, Quaternion.Euler(0, prefabAngle, 0), this.transform);
            }
        }
    }

    public GameObject PrefabToPlace;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPos = hit.point;
                // 计算鼠标点击位置的六边形坐标
                HexCoord clickedCoord = GetHexCoordFromWorldPos(hitPos);

                if (BackgroundCoords.Contains(clickedCoord) && !ActiveCoords.Contains(clickedCoord))
                {
                    ActiveCoords.Add(clickedCoord);
                    BackgroundCoords.Remove(clickedCoord);

                    // 在点击位置实例化Prefab
                    Vector3 spawnPos = HexCoord.ToWorldPos(clickedCoord, HexSize);

                    GameObject popObject = PrefabStack.Pop();
                    if (popObject != null)
                    {
                        float _y = popObject.transform.rotation.eulerAngles.y - 90;
                        Instantiate(popObject, spawnPos, Quaternion.Euler(0, prefabAngle + _y, 0));
                        //Debug.Log(prefabAngle + " " + _y);
                        GameObject placeObject = Instantiate(popObject, spawnPos, Quaternion.Euler(0, prefabAngle + _y, 0));

                        Vector3 newPosition = placeObject.transform.position;
                        newPosition.y += Mathf.PerlinNoise(spawnPos.x, spawnPos.z) * 0.5f;
                        //Debug.Log(spawnPos.x + " " + spawnPos.z + " " + Mathf.PerlinNoise(spawnPos.x, spawnPos.z));

                        placeObject.transform.position = newPosition;

                        SetLayer(placeObject, 0);

                        foreach (HexCoord hexCell in GetNeighbors(clickedCoord))
                        {
                            if (!ActiveCoords.Contains(hexCell) && !BackgroundCoords.Contains(hexCell))
                            {
                                CreateBackgroundCoord(hexCell, clickedCoord.type);
                            }

                        }
                    }
                }
            }
        } else if (Input.GetMouseButtonDown(1))
        {
            PrefabStack.RotateTop();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PrefabStack.InitializeStack(5);
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

    public void CreateBackgroundCoord(HexCoord hexCell, int type)
    {
        BackgroundCoords.Add(hexCell);
        hexCell.type = type;

        Vector3 pos = HexCoord.ToWorldPos(hexCell, HexSize);

        pos.y += Mathf.PerlinNoise(pos.x, pos.z) * 0.5f - 0.05f;
        Instantiate(HexPrefab, pos, Quaternion.Euler(0, prefabAngle, 0), this.transform);
    }

    HexCoord GetHexCoordFromWorldPos(Vector3 worldPos)
    {
        float q = (worldPos.x * 2 / 3) / HexSize;
        float r = (-worldPos.x / 3 + Mathf.Sqrt(3) / 3 * worldPos.z) / HexSize;
        return RoundHexCoord(q, r);
    }

    HexCoord RoundHexCoord(float q, float r)
    {
        int rx = Mathf.RoundToInt(q);
        int ry = Mathf.RoundToInt(-q - r);
        int rz = Mathf.RoundToInt(r);

        float x_diff = Mathf.Abs(rx - q);
        float y_diff = Mathf.Abs(ry + q + r);
        float z_diff = Mathf.Abs(rz - r);

        if (x_diff > y_diff && x_diff > z_diff)
            rx = -ry - rz;
        else if (y_diff > z_diff)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        return new HexCoord(rx, ry);
    }

    public HexCoord GetMouseHexCoord()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return GetHexCoordFromWorldPos(hit.point);
        }
        return new HexCoord(0, 0); // Default value
    }

    public Vector3 GetWorldPosFromHexCoord(HexCoord hexCoord)
    {
        return HexCoord.ToWorldPos(hexCoord, HexSize);
    }

    public HexCoord[] GetNeighbors(HexCoord hexCoord)
    {
        HexCoord[] directions = new HexCoord[]
        {
        new HexCoord(1, 0), new HexCoord(1, -1), new HexCoord(0, -1),
        new HexCoord(-1, 0), new HexCoord(-1, 1), new HexCoord(0, 1)
        };

        HexCoord[] neighbors = new HexCoord[6];
        for (int i = 0; i < 6; i++)
        {
            neighbors[i] = new HexCoord(hexCoord.X + directions[i].X, hexCoord.Y + directions[i].Y);
        }
        return neighbors;
    }

}
