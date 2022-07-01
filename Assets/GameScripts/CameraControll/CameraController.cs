using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float edgeSize = 30;
    private float moveSize = 30f;
    private float scrollSpeed = 20f;

    public Vector2 mapLimits;
    private float max_y_axis = 120f;
    private float min_y_axis = 40f;

    private Vector3 cameraPos;
    void Update()
    {
        cameraPos = transform.position;
        if (Input.mousePosition.y >= Screen.height - edgeSize)
        {
            cameraPos.z += moveSize * Time.deltaTime;
        }
        if (Input.mousePosition.y <= edgeSize)
        {
            cameraPos.z -= moveSize * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - edgeSize)
        {
            cameraPos.x += moveSize * Time.deltaTime;
        }
        if (Input.mousePosition.x <= edgeSize)
        {
            cameraPos.x -= moveSize * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cameraPos.y -= scroll * scrollSpeed * 150f * Time.deltaTime;
        
        cameraPos.x = Mathf.Clamp(cameraPos.x, -mapLimits.x,mapLimits.x);
        cameraPos.y = Mathf.Clamp(cameraPos.y, min_y_axis,max_y_axis);
        cameraPos.z = Mathf.Clamp(cameraPos.z, -mapLimits.y,mapLimits.y);
        
        transform.position = cameraPos;
    }
}
