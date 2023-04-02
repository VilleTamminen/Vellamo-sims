using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    //Enables selecting one object at a time and gives them an outline
    public GameObject selectedObject;
    public Text objectNameText;
    public GameObject selectUI; //SelectedObjectPanel

    private BuildingManager buildingManager;


    private void Start()
    {
        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();
        selectUI.SetActive(false); //Hide panel if not already unactive in scene

        Outline[] outlineScrips = FindObjectsOfType<Outline>();
        foreach(Outline outline in outlineScrips)
        {
            outline.enabled= false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.gameObject.layer == 6)
                {
                    Select(hit.collider.gameObject);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Deselect();
        }
    }
    public void Select(GameObject obj)
    {
        if (obj == selectedObject) { return; }
        if (selectedObject != null) { Deselect(); }

      //  Outline outline = obj.GetComponent<Outline>();
        if (obj.GetComponent<Outline>() == null && obj.transform.tag != "Untagged") { obj.AddComponent<Outline>(); }
        else { obj.GetComponent<Outline>().enabled = true; }
      
        objectNameText.text = obj.name;
        if (obj.transform.root.gameObject.tag != "MeasureTool")
        {
            selectedObject = obj.transform.root.gameObject;
        }
        else
        {
            //measureTool has children that must be moved separately
            selectedObject = obj;
        }
        selectUI.SetActive(true);
    }

    void Deselect()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Outline>().enabled = false;
            //Turn off Outlines in children
            Outline[] childOutlines = selectedObject.GetComponentsInChildren<Outline>();
            if (childOutlines.Length > 0)
            {
                foreach (Outline childOutline in childOutlines)
                {
                    childOutline.enabled = false;
                }
            }
            selectUI.SetActive(false);

            if (selectedObject.transform.root.gameObject.tag != "MeasureTool")
            {
                //Hide distance measurePoints
                GameObject[] measurePoints = selectedObject.transform.root.GetComponentInChildren<CheckBuildablePlacement>().measurePoints;
                foreach (GameObject obj in measurePoints)
                {
                    obj.SetActive(false);
                }
            }
            selectedObject = null;
        }
    }

    public void Move()
    {
        //This worked until child objects needed to be able to move separately, like in MeasureTool
        // buildingManager.pendingObject = selectedObject.transform.root.gameObject;

        if (selectedObject.transform.root.gameObject.tag != "MeasureTool")
        {
            buildingManager.pendingObject = selectedObject.transform.root.gameObject;
        }
        else
        {
            //measureTool has children that must be moved separately
            buildingManager.pendingObject = selectedObject;
        }
    }
    public void Delete()
    {
        GameObject objectToDestroy = selectedObject.transform.root.gameObject;
        Deselect();
        Destroy(objectToDestroy);
    }

    void FixedUpdate()
    {
        if (selectedObject != null && selectedObject.transform.root.tag != "MeasureTool") { UpdateDistanceLines(); }
    }
    void UpdateDistanceLines()
    {
        /*
        measurePoints[0] = selectedObject.transform.root.Find("measurePoint1").gameObject;
        measurePoints[1] = selectedObject.transform.root.Find("measurePoint2").gameObject;
        measurePoints[2] = selectedObject.transform.root.Find("measurePoint3").gameObject;
        measurePoints[3] = selectedObject.transform.root.Find("measurePoint4").gameObject;
        */
        GameObject[] measurePoints = selectedObject.transform.root.GetComponentInChildren<CheckBuildablePlacement>().measurePoints;

        //4 transform where rays are shot outwards to show distance to closest object
        //Update distance text and visualize path with Line Renderer in all measurePoints
        //measure points must face away from object (transform.forward)
        foreach (GameObject obj in measurePoints)
        {
            obj.SetActive(true);
            Ray distanceRay = new Ray(obj.transform.position, obj.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(distanceRay, out hit))
            {
                //.ToString(".0####") amount of # means how many decimals show in float
                obj.GetComponentInChildren<TextMeshPro>().text = hit.distance.ToString(".0#") + "m";
                obj.GetComponent<LineRenderer>().SetPosition(0, obj.transform.position);
                obj.GetComponent<LineRenderer>().SetPosition(1, hit.point);
            }
        }
    }

}
