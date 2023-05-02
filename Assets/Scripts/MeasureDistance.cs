using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Used by MeasureTool object
public class MeasureDistance : MonoBehaviour
{
    //Has a pair of points and calculates distance between them
    public Transform point1;
    public Transform point2;
    private float distance = 0;
    public TextMeshPro distanceText;
    public LineRenderer line;

    private void Start()
    {
        if(point1 == null)
        {
            point1 = gameObject.transform.Find("Point1");
        }
        if (point2 == null)
        {
            point2 = gameObject.transform.Find("Point2");
        }
    }

    private void Update()
    {
       //Function is called in Update() so that text rotates and faces camera side always. Distance is also updated.
        measureDistance();
    }

    public void measureDistance()
    {
        distance = Vector3.Distance(point1.transform.position, point2.transform.position);
        //.ToString(".0####") amount of # means how many decimals show in float
        distanceText.text = distance.ToString(".0#") + "m";

        //Get middle position between points and keep text object there
        Vector3 midpoint = new Vector3((point1.position.x + point2.position.x) / 2.0f, (point1.position.y + point2.position.y) / 2.0f, (point1.position.z + point2.position.z) / 2.0f); 
        distanceText.transform.position = midpoint;

        //Rotate towards point1 and align rotation between points
        distanceText.transform.LookAt(point1.position);
        // Euler angles are easier to deal with. You could use Quaternions here also.
        // C# requires you to set the entire rotation variable. You can't set the individual x and z (UnityScript can), so you make a temp Vec3 and set it back
        Vector3 eulerAngles = distanceText.transform.rotation.eulerAngles;
        //The text side is always kept towards camera, so it is always readable
        Vector3 cameraDir = (distanceText.transform.position - Camera.main.transform.position).normalized;
        float cameraAngle = Vector3.SignedAngle(cameraDir, distanceText.transform.forward, Vector3.up);
        if (cameraAngle >= 0)
        {
            eulerAngles.y -= 90;
        }
        else {
            eulerAngles.y += 90; 
        }
        //Don't rotate on x,z axis
        eulerAngles.x = 0; 
        eulerAngles.z = 0;
        distanceText.transform.rotation = Quaternion.Euler(eulerAngles);

        UpdateLine(point1.position, point2.position);
    }

    /// <summary>
    /// Updates visual line renderer between 2 points
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    private void UpdateLine(Vector3 point1, Vector3 point2)
    {
        line.SetPosition(0, point1);
        line.SetPosition(1, point2);
    }
}
