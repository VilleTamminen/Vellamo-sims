using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    //Enables selecting one object at a time and gives them an outline
    public GameObject selectedObject;

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
    void Select(GameObject obj)
    {
        if (obj == selectedObject) { return; }
        if(selectedObject != null) { Deselect(); }
        Outline outline = obj.GetComponent<Outline>();
        if (outline == null) { obj.AddComponent<Outline>(); }
        else { outline.enabled = true; }
        selectedObject = obj;
    }
    void Deselect()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Outline>().enabled = false;
            selectedObject = null;
        }
    }
}
