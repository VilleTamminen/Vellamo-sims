using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CheckBuildablePlacement : MonoBehaviour
{
    //Checks if buildable object is not inside other Objects with tag
    private BuildingManager buildingManager;
    //layers that prevent building placement. 6 = Buildable, 7 = Wall
    public LayerMask[] obstacleLayers = { 6, 7 };
    public GameObject[] measurePoints = new GameObject[4];
    public bool hasMeasurePoints = false;

    void Start()
    {
        buildingManager = BuildingManager.Instance.GetComponent<BuildingManager>();
            //GameObject.Find("BuildingManager").GetComponent<BuildingManager>();
        if(buildingManager == null)
        {
            Debug.LogError("BuildingManager is missing from scene!!!");
        }

        SetMeasurePoints();
    }

    private void OnTriggerStay(Collider other)
    {
        if (obstacleLayers.Contains(other.gameObject.layer))
        {
            buildingManager.canPlace = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (obstacleLayers.Contains(other.gameObject.layer))
        {
            buildingManager.canPlace = true;
        }
    }

    private void SetMeasurePoints()
    {
        //If measurePoint exists, then set and then hide them. Used by selectManager to visualize distance to other objects.
        if (transform.root.Find("measurePoint1") != null)
        {
            hasMeasurePoints = true;
            measurePoints[0] = transform.root.Find("measurePoint1").gameObject;
            measurePoints[1] = transform.root.Find("measurePoint2").gameObject;
            measurePoints[2] = transform.root.Find("measurePoint3").gameObject;
            measurePoints[3] = transform.root.Find("measurePoint4").gameObject;
            foreach (GameObject obj in measurePoints)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            hasMeasurePoints = false;
        }
    } 
}
