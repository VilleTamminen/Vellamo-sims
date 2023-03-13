using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Works in orthographic projection but not in perspective projection
public class CameraMove : MonoBehaviour
{
    private Vector3 origin;
    private Vector3 difference;
    private Vector3 resetCamera;

    private bool drag = false;

    // Start is called before the first frame update
    private void Start()
    {
        resetCamera = Camera.main.transform.position;
    }

    private void LateUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if(drag == false)
            {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }

        if(drag)
        {
            Camera.main.transform.position = origin - difference;
        }

        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = resetCamera;
        }
    }
}
