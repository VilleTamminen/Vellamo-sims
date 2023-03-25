using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000))
            {
                if(hit.collider.gameObject.layer == 6)
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
        if(selectedObject != null) { Deselect(); }
        Outline outline = obj.GetComponent<Outline>();
        if (outline == null) { obj.AddComponent<Outline>(); }
        else { outline.enabled = true; }

        objectNameText.text = obj.name;
        if(obj.transform.root.gameObject.tag != "MeasureTool"){
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
            selectedObject.transform.parent = null;
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
}
