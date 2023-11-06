using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
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

    public GameObject indicatorPrefab;

    private void Awake()
    {
        //GenerateGrid();
        CreateBackgroundCoord(HexCoord.FromOffset(1, 2));
        CreateBackgroundCoord(HexCoord.FromOffset(0, 2));
        CreateBackgroundCoord(HexCoord.FromOffset(-1, 2));
        //CreateBackgroundCoord(HexCoord.FromOffset(-1, 2));
        CreateBackgroundCoord(HexCoord.FromOffset(-1, 2));
        CreateBackgroundCoord(HexCoord.FromOffset(-1, 1));
        //CreateBackgroundCoord(HexCoord.FromOffset(-3, 0));
        //CreateBackgroundCoord(HexCoord.FromOffset(-1, -2));
        //CreateBackgroundCoord(HexCoord.FromOffset(-2, -1));
        CreateBackgroundCoord(HexCoord.FromOffset(0, -2));
        CreateBackgroundCoord(HexCoord.FromOffset(1, -3));
        CreateBackgroundCoord(HexCoord.FromOffset(2, -3));
        CreateBackgroundCoord(HexCoord.FromOffset(3, -3));
        CreateBackgroundCoord(HexCoord.FromOffset(3, -2));
        CreateBackgroundCoord(HexCoord.FromOffset(3, -1));

        PrefabStack = FindObjectOfType<PrefabStack>();
        PrefabStack.prefabs = prefabs;
        //PrefabStack.AmountToSpawn = AmountToSpawn;
        PrefabStack.InitializeStack(AmountToSpawn);

    }

    public void Start()
    {
    }


    public GameObject PrefabToPlace;
    bool canPlace;

    private GameObject currentPreview;
    GameObject previewPrefab;

    public bool isHovering = false;

    void Update()
    {
        // 获取当前鼠标所在的格子坐标
        HexCoord newCoord = GetMouseHexCoord();
        isHovering = false;

        Debug.Log(newCoord.X + " " + newCoord.Y);

        if (PrefabStack.Count() > 0)
        {

            PrefabCell cellToPlace = PrefabStack.PreviewPop().GetComponent<PrefabCell>();
            canPlace = true;


            if (BackgroundCoords.Contains(newCoord) && !ActiveCoords.Contains(newCoord))
            {
                isHovering = true;
                Vector3 spawnPos = HexCoord.ToWorldPos(newCoord, HexSize);

                cellToPlace.HideAllEdges();

                string edgeList = "";

                // 检查所有邻居
                foreach (HexCoord neighbor in GetNeighbors(newCoord))
                {
                    if (ActiveCoords.Contains(neighbor))
                    {
                        PrefabCell neighborCell = ActiveCoords.Find(coord => coord.Equals(neighbor)).prefabCell;

                        int? _edge = newCoord.GetNeighborCellEdgeIndex(neighbor);

                        edgeList += _edge + ", ";

                        //Debug.Log(_edge);

                        if (_edge != null)
                        {
                            int edge = (int)_edge;
                            if (!PrefabCell.CanMatch(cellToPlace.GetTypeForEdge(edge), neighborCell.GetTypeForEdge(PrefabCell.GetOppositeEdgeIndex(edge))))
                            {
                                canPlace = false;
                                //Debug.Log("Self:" + _edge + "," + cellToPlace.GetTypeForEdge(edge) + " Other: " + PrefabCell.GetOppositeEdgeIndex(edge) + ", "+ neighborCell.GetTypeForEdge(PrefabCell.GetOppositeEdgeIndex(edge)));
                                //cellToPlace.edgePlaceable[edge] = false;
                                cellToPlace.ShowEdge(edge);

                            }
                            else
                            {
                                //cellToPlace.edgePlaceable[edge] = true;
                            }
                        } else
                        {
                            //Debug.Log("Not neighbor!");
                        }
                    }
                }
                //Debug.Log(edgeList);

                // 如果当前没有预览，或者预览的位置与新位置不同，则创建或移动预览
                if (currentPreview == null || currentPreview.transform.position != spawnPos)
                {
                    // 如果已经有预览，则先销毁
                    if (currentPreview != null)
                    {
                        Destroy(currentPreview);
                    }

                    previewPrefab = PrefabStack.PreviewPop();
                    if (previewPrefab != null)
                    {
                        //Debug.Log(previewPrefab.transform.rotation.eulerAngles.y);
                        float _y = previewPrefab.transform.GetChild(1).rotation.eulerAngles.y - 90;

                        //currentPreview = Instantiate(previewPrefab, spawnPos, Quaternion.Euler(0, prefabAngle + _y, 0));
                        currentPreview = Instantiate(previewPrefab, spawnPos, Quaternion.Euler(0, prefabAngle, 0));
                        currentPreview.transform.GetChild(1).rotation = Quaternion.Euler(0, prefabAngle + _y, 0);


                        Vector3 newPosition = currentPreview.transform.position;
                        newPosition.y += Mathf.PerlinNoise(spawnPos.x, spawnPos.z) * 0.5f;
                        //Debug.Log(spawnPos.x + " " + spawnPos.z + " " + Mathf.PerlinNoise(spawnPos.x, spawnPos.z));
                        currentPreview.transform.position = newPosition;
                        SetLayer(currentPreview, 0);
                    }
                }
            }
            else
            {
                if (currentPreview != null)
                {
                    Destroy(currentPreview);
                    currentPreview = null;
                }

                canPlace = false;
            }


            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                HexCoord clickedCoord = newCoord;

                if (BackgroundCoords.Contains(clickedCoord) && !ActiveCoords.Contains(clickedCoord))
                {

                    // 在点击位置实例化Prefab
                    Vector3 spawnPos = HexCoord.ToWorldPos(clickedCoord, HexSize);

                    GameObject popObject = PrefabStack.Pop();
                    if (popObject != null)
                    {
                        float _y = popObject.transform.GetChild(1).rotation.eulerAngles.y - 90;
                        //Instantiate(popObject, spawnPos, Quaternion.Euler(0, prefabAngle + _y, 0));
                        //Debug.Log(prefabAngle + " " + _y);
                        //GameObject placeObject = Instantiate(popObject, spawnPos, Quaternion.Euler(0, prefabAngle + _y, 0));
                        GameObject placeObject = Instantiate(popObject, spawnPos, Quaternion.Euler(0, prefabAngle, 0));
                        placeObject.transform.GetChild(1).rotation = Quaternion.Euler(0, prefabAngle + _y, 0);


                        Vector3 newPosition = placeObject.transform.position;
                        newPosition.y += Mathf.PerlinNoise(spawnPos.x, spawnPos.z) * 0.5f;
                        //Debug.Log(spawnPos.x + " " + spawnPos.z + " " + Mathf.PerlinNoise(spawnPos.x, spawnPos.z));

                        placeObject.transform.position = newPosition;

                        SetLayer(placeObject, 0);

                        BackgroundCoords.Remove(clickedCoord);

                        clickedCoord.prefabCell = placeObject.GetComponent<PrefabCell>();

                        ActiveCoords.Add(clickedCoord);
                        placeObject.GetComponent<PrefabCell>().hexCoord = clickedCoord;

                        foreach (HexCoord hexCell in GetNeighbors(clickedCoord))
                        {
                            if (!ActiveCoords.Contains(hexCell) && !BackgroundCoords.Contains(hexCell))
                            {
                                CreateBackgroundCoord(hexCell);
                            }
                            //else if (ActiveCoords.Contains(hexCell))
                            //{
                            //    hexCell.prefabCell.gameObject.transform.DOShakePosition(0.3f, new Vector3(0.1f, 0, 0), 10, 0);
                            //}

                        }
                    }

                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                PrefabStack.RotateTop();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PrefabStack.InitializeStack(5);
        }

    }

    public void placePlate(GameObject prefab, HexCoord clickedCoord)
    {

        Vector3 spawnPos = HexCoord.ToWorldPos(clickedCoord, HexSize);

        float _y = prefab.transform.GetChild(1).rotation.eulerAngles.y - 90;
        //Instantiate(popObject, spawnPos, Quaternion.Euler(0, prefabAngle + _y, 0));
        //Debug.Log(prefabAngle + " " + _y);
        //GameObject placeObject = Instantiate(popObject, spawnPos, Quaternion.Euler(0, prefabAngle + _y, 0));
        GameObject placeObject = Instantiate(prefab, spawnPos, Quaternion.Euler(0, prefabAngle, 0));
        placeObject.transform.GetChild(1).rotation = Quaternion.Euler(0, prefabAngle + _y, 0);


        Vector3 newPosition = placeObject.transform.position;
        newPosition.y += Mathf.PerlinNoise(spawnPos.x, spawnPos.z) * 0.5f;
        //Debug.Log(spawnPos.x + " " + spawnPos.z + " " + Mathf.PerlinNoise(spawnPos.x, spawnPos.z));

        placeObject.transform.position = newPosition;

        SetLayer(placeObject, 0);

        BackgroundCoords.Remove(clickedCoord);

        clickedCoord.prefabCell = placeObject.GetComponent<PrefabCell>();

        ActiveCoords.Add(clickedCoord);
        placeObject.GetComponent<PrefabCell>().hexCoord = clickedCoord;

        foreach (HexCoord hexCell in GetNeighbors(clickedCoord))
        {
            if (!ActiveCoords.Contains(hexCell) && !BackgroundCoords.Contains(hexCell))
            {
                CreateBackgroundCoord(hexCell);
            }

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

    public void CreateBackgroundCoord(HexCoord hexCell)
    {
        BackgroundCoords.Add(hexCell);

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
        int layerMask = ~(1 << LayerMask.NameToLayer("IgnoreRaycast"));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
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
