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
        selectedObject = obj;
        selectUI.SetActive(true);
    }

    void Deselect()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Outline>().enabled = false;
            selectUI.SetActive(false);
            selectedObject = null;
        }
    }

    public void Move()
    {
        buildingManager.pendingObject = selectedObject;
    }
    public void Delete()
    {
        GameObject objectToDestroy = selectedObject;
        Deselect();
        Destroy(objectToDestroy);
    }
}
