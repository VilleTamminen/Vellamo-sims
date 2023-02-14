using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    private SelectManager selectManager;
    public GameObject[] objects;
    public GameObject pendingObject;

    private Vector3 pos;
    private RaycastHit hit;
    public bool canPlace = true;

    public int floorIndex = 0;
    private float floorHeight = 0.0f;

    //layerMask where object can be moved and placed (Ground = 3)
    [SerializeField] private LayerMask layerMask;

    public float gridSize;
    bool gridOn = true;
    [SerializeField] private Toggle gridToggle;
    [SerializeField] private Toggle wallToggle;

    private float rotateAmount = 5f;
    //used for showing visually if object can be placed
    [SerializeField] private Material[] materials;
    private Material originalMaterial;

    void Start()
    {
        //Must be true at the start
        canPlace = true;
        selectManager = GameObject.Find("SelectManager").GetComponent<SelectManager>();
    }
    void Update()
    {
        UpdateFloor();

        if (pendingObject != null)
        {
            //If grid is on, round position to nearest grid position
            if (gridOn)
            {
                //without floorHeight,objects keep moving towards camera, because they are valid building grounds.
                //This can also be fixed by changing their layerMask during building phase.
                pendingObject.transform.position = new Vector3(RoundToNearestGrid(pos.x), floorHeight, RoundToNearestGrid(pos.z));
            }
            else
            {
                pendingObject.transform.position = new Vector3(pos.x, floorHeight, pos.z);
            }

            UpdateMaterials();
            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceObject();
            }

            //Rotate object based on positive/negative inputs from either mouse wheel or E/Q buttons
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                RotateObject(true, true);
            }
            else if(Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                RotateObject(false, true);
            }
            if (Input.GetAxis("Rotate") > 0)
            {
                RotateObject(true, false);
            }
            else if (Input.GetAxis("Rotate") < 0)
            {
                RotateObject(false, false);
            }
        }
    }

    private void UpdateFloor()
    {
        //Building has more than one floor, with possible different heights.
        //If building on top of buildable objects is wanted, then adding a check for buildable layerMask
        //and fecthing it's height is reguired. This means all buildables would required a height parameter.
        if (floorIndex == 0)
        {
            floorHeight = 0.0f;
        }
    }
    public void SelectObject(int index)
    {
        pendingObject = Instantiate(objects[index], pos, transform.rotation);
        selectManager.Select(pendingObject);
    }
    public void PlaceObject()
    {
        //update object material with it's original material
        pendingObject.GetComponent<MeshRenderer>().material = originalMaterial;
        originalMaterial = null;
        pendingObject = null;
    }

    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            pos = hit.point;
        }
    } 

    public void ToggleGrid()
    {
        if(gridToggle.isOn)
        {
            gridOn = true;
        }
        else { gridOn = false; }
    }

    public void ToggleWalls()
    {
        float wallTransparency = 1.0f;
        if (wallToggle.isOn)
        {
            wallTransparency = 1.0f;
        }
        else { wallTransparency = 0.35f; }

        //gather all objects tagged with "Wall"
        var walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (var wall in walls)
        {
            //set material transparency
            Color wallColor = wall.GetComponent<MeshRenderer>().material.color;
            wallColor.a = wallTransparency; 
            wall.GetComponent<MeshRenderer>().material.color = wallColor;
        }
    }

    float RoundToNearestGrid(float pos)
    {
        /* this method gets the remainder of the position and the grid size after they are divided, 
         * subtracting it to snap it downwards, and then checks to see if it needs to snap 
         * upwards instead so that it rounds to the nearest value, not just rounding down.*/
        float xDiff = pos % gridSize;
        pos -= xDiff;
        if(xDiff > (gridSize / 2))
        {
            pos += gridSize;
        }
        return pos;
    }

    void RotateObject(bool directionUp, bool isMouse)
    {
        float currentRotateAmount = 1;

        if (isMouse == false)
        {
            //mouse works fine with default rotate amount, but E/Q buttons spin it too fast (they need lower speed)
            currentRotateAmount = 1;
        }
        else { currentRotateAmount = rotateAmount; }

        if (directionUp == true)
        {
            pendingObject.transform.Rotate(Vector3.up, currentRotateAmount);
        }
        else { pendingObject.transform.Rotate(Vector3.down, currentRotateAmount); }
    }

    void UpdateMaterials()
    {
        //Update materials to visually show if object can be placed
        if (originalMaterial == null) 
        {
            originalMaterial = pendingObject.GetComponent<MeshRenderer>().material;
        }
        if (canPlace)
        {
            pendingObject.GetComponent<MeshRenderer>().material = materials[0];
        }
        else { pendingObject.GetComponent<MeshRenderer>().material = materials[1]; }
    }
}
