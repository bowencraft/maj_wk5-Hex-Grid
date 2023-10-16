using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinInCamera : MonoBehaviour
{

    public Camera cam;
    public Vector3 offset = new Vector3(-0.15f, -0.1f, 1); // 调整这个偏移量使3D物体出现在所需的位置

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        Vector3 screenPos = new Vector3(Screen.width, 0, cam.nearClipPlane);
        Vector3 worldPosition = cam.ScreenToWorldPoint(screenPos);
        transform.position = worldPosition + offset;
    }
}
