using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckBuildablePlacement : MonoBehaviour
{
    //Checks if buildable object is not inside other Objects with tag
    private BuildingManager buildingManager;
    //layers that prevent building placement. 6 = Buildable, 7 = Wall
    public LayerMask[] obstacleLayers = { 6, 7 };

    void Start()
    {
        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();
        if(buildingManager == null)
        {
            Debug.LogError("BuildingManager is missing from scene!!!");
        }
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
}
