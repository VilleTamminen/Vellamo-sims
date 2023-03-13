using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tutorial from Brackeys: https://www.youtube.com/watch?v=cfjLQrMGEb4 
//Works with perspective camera
//Place on empty parent object and make Camera child object = transform.Translate() movement ignores rotation when turning camera.
public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f;
    public float panBorderThickness = 10f; //mouse being X pixels away from screen border activates pan
    public Vector2 panLimit; //make borders how far pan can go. Must be recalculated when Vellamo map size is ready
    public float cameraRotateSpeed = 10f;

    private SelectManager selectManager;

    private void Start()
    {
        selectManager = GameObject.Find("SelectManager").GetComponent<SelectManager>();
    }
    void Update()
    {
        float xMovement = 0;
        float zMovement = 0;
        if (Input.GetKey("w") || Input.GetKey("up") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            zMovement += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.GetKey("down") || Input.mousePosition.y <= panBorderThickness)
        {
            zMovement -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.GetKey("right") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            xMovement += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.GetKey("left") || Input.mousePosition.x <= panBorderThickness)
        {
            xMovement -= panSpeed * Time.deltaTime;
        }
     
        transform.Translate(new Vector3(xMovement, 0,0) * panSpeed, Space.Self);
        transform.Translate(new Vector3(0,0, zMovement) * panSpeed, Space.Self);
         CheckPanLimit();

        //If no objects are selected then camera can be rotated.
        if (selectManager.selectedObject == null)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                transform.Rotate(0, scroll * cameraRotateSpeed, 0, Space.World);
            }
            if (Input.GetAxis("Rotate") > 0)
            {
                transform.Rotate(Vector3.up, 1.0f);
            }
            else if (Input.GetAxis("Rotate") < 0)
            {
                transform.Rotate(Vector3.down, 1.0f);
            }
        }

    }

    //Doesn't allow camera to exit area panLimit area
    void CheckPanLimit()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        transform.position = pos;
    }
}
