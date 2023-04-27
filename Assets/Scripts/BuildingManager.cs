using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    private SelectManager selectManager;
    public GameObject[] objects;
    //if pending object is not null, then that object is constantly moved with using mouse position on screen raycast.
    public GameObject pendingObject;

    //Position where raycast hits
    private Vector3 pos;
    private RaycastHit hit;
    public bool canPlace = true;

    //layerMask where object can be moved and placed (Ground = 3, Buildable = 6)
    [SerializeField] private LayerMask layerMask;

    public float gridSize;
    bool gridOn = false;
    [SerializeField] private Toggle gridToggle;
    [SerializeField] private Toggle wallToggle;

    private float rotateAmount = 5f;
    //used for showing visually if object can be placed
    [SerializeField] private Material[] materials;
    private Material originalMaterial;

    private static BuildingManager instance;
    public static BuildingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<BuildingManager>();
            }
            return instance;
        }
    }

    void Start()
    {
        //Must be true at the start
        canPlace = true;
        selectManager = SelectManager.Instance;
    }
    void Update()
    {
        if (pendingObject != null)
        {
            SelectManager.Instance.isMoving = true;
            //If grid is on, round position to nearest grid position
            if (gridOn)
            {
                //without floorHeight,objects keep moving towards camera, because they are valid building grounds.
                //This can also be fixed by changing their layerMask during building phase.
                pendingObject.transform.position = new Vector3(RoundToNearestGrid(pos.x), pos.y, RoundToNearestGrid(pos.z));
            }
            else
            {
                pendingObject.transform.position = new Vector3(pos.x, pos.y, pos.z);
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
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
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

    /// <summary>
    /// Buttons from ObjectPanel use this to spawn objects with index.
    /// </summary>
    public void SelectObject(int index)
    {
        pendingObject = Instantiate(objects[index], pos, transform.rotation); 
        //Set correct name without (Clone), so that Save system works with Resources.Load(name)
        pendingObject.name = objects[index].name;
        //Makinbg object untagged allows us to ignore it during raycasting
        // pendingObject.gameObject.tag = "Untagged";
        selectManager.Select(pendingObject);
    }

    /// <summary>
    /// Only changes object materials.
    /// </summary>
    public void PlaceObject()
    {
        //update object material and it's children material with the original material
        if (pendingObject.GetComponent<MeshRenderer>() != null)
        {
            pendingObject.GetComponent<MeshRenderer>().material = originalMaterial;
        }
        MeshRenderer[] childMeshRenderers = pendingObject.GetComponentsInChildren<MeshRenderer>();
        if (childMeshRenderers.Length > 0)
        {
            foreach (MeshRenderer rend in childMeshRenderers)
            {
                if (rend.transform.gameObject.tag == "Buildable" || rend.transform.gameObject.tag == "Wall")
                {
                    rend.material = originalMaterial;
                }
            }
        }

        //Give back original tag
        // pendingObject.gameObject.tag = "Buildable";
        SelectManager.Instance.isMoving = false;
        originalMaterial = null;
        pendingObject = null;
    }

    void FixedUpdate()
    {
        //Normal raycast. Doesn't allow placing objects on top of other Buildables.
        /* Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         //Updates position where mouse raycast hits object
         if (Physics.Raycast(ray, out hit, 1000, layerMask))
         {
             pos = hit.point;
         }
         */

        //RaycastAll
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] HitObjects = Physics.RaycastAll(ray, 1000, layerMask);

        //Get shortest distance raycast hit
        RaycastHit shortestHit = new RaycastHit();
        float shortestDist = Mathf.Infinity;
        bool foundHit = false;
        if (pendingObject != null)
        {
            SelectManager.Instance.isMoving = true;
            for (int i = 0; i < HitObjects.Length; i++)
            {
                if (HitObjects[i].transform.root.gameObject == pendingObject.transform.root.gameObject || HitObjects[i].transform.gameObject.tag == "MeasureTool")
                {
                    //Ignore pendingObject and MeasureTool, since it needs its children to be able to move separately.
                    // Debug.Log("hitobject: " + HitObjects[i].transform.root.gameObject + "pending obj: " + pendingObject.transform.root.gameObject);
                }
                else
                {
                    //If hit object is on layer 6 (Buildable) then it has to have tag "Buildable". Pending object is Untagged during placement.
                    if ((HitObjects[i].transform.root.tag == "Buildable" && HitObjects[i].transform.root.gameObject.layer == 6) ||
                        (HitObjects[i].transform.root.tag != "Buildable" && HitObjects[i].transform.root.gameObject.layer != 6))
                    {

                        if (Vector3.Distance(Camera.main.transform.position, HitObjects[i].point) < shortestDist)
                        {
                            shortestDist = Vector3.Distance(Camera.main.transform.position, HitObjects[i].point);
                            shortestHit = HitObjects[i];
                            foundHit = true;
                            // Debug.Log("hitobject: " + HitObjects[i].transform.root.gameObject + "pending obj: " + pendingObject.transform.root.gameObject);
                        }
                    }
                }
            }
            if (foundHit)
            {
                pos = shortestHit.point;
            }
        }

    }

    public void ToggleGrid()
    {
        if (gridToggle.isOn)
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
        if (xDiff > (gridSize / 2))
        {
            pos += gridSize;
        }
        return pos;
    }

    void RotateObject(bool directionUp, bool isMouse)
    {
        //This second check for pendingObject prevents errors somehow.
        if (pendingObject != null)
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
    }

    void UpdateMaterials()
    {
        bool hasParentMaterial = false;
        if (pendingObject.GetComponent<MeshRenderer>() != null || pendingObject.GetComponentInChildren<MeshRenderer>() != null)
        {
            //Update materials to visually show if object can be placed. Also for children.
            if (pendingObject.GetComponent<MeshRenderer>() != null && originalMaterial == null)
            {
                originalMaterial = pendingObject.GetComponent<MeshRenderer>().material;
                hasParentMaterial = true;
            }
            else if (pendingObject.GetComponentInChildren<MeshRenderer>() != null && originalMaterial == null)
            {
                originalMaterial = pendingObject.GetComponentInChildren<MeshRenderer>().material;
                hasParentMaterial = false;
            }
            //Change children's material too if they are Buildables or Walls
            MeshRenderer[] childMeshRenderers = pendingObject.GetComponentsInChildren<MeshRenderer>();
            if (canPlace)
            {
                if(hasParentMaterial)
                {
                    pendingObject.GetComponent<MeshRenderer>().material = materials[0];
                }

                if (childMeshRenderers.Length > 0)
                {
                    foreach (MeshRenderer rend in childMeshRenderers)
                    {
                        if (rend.transform.gameObject.tag == "Buildable" || rend.transform.gameObject.tag == "Wall")
                        {
                            rend.material = materials[0];
                        }
                    }
                }
            }
            else
            {
                if (hasParentMaterial)
                {
                    pendingObject.GetComponent<MeshRenderer>().material = materials[1];
                }
                else
                {
                    pendingObject.GetComponentInChildren<MeshRenderer>().material = materials[1];
                }
                if (childMeshRenderers.Length > 0)
                {
                    foreach (MeshRenderer rend in childMeshRenderers)
                    {
                        if (rend.transform.gameObject.tag == "Buildable" || rend.transform.gameObject.tag == "Wall")
                        {
                            rend.material = materials[1];
                        }
                    }
                }
            }
        }
    }

}
