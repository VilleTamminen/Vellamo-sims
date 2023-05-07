using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Old Bugs: Last selected object's customized scale can affect newly instantiated object scale.

public class SelectManager : MonoBehaviour
{
    //Enables selecting one object at a time and gives them an outline
    public GameObject selectedObject;
    public TMP_Text objectNameText;
    public GameObject selectUI; //SelectedObjectPanel
    private BuildingManager buildingManager;

    public bool isMoving = false;

    //Input fields allow custom scales for objects
    public TMP_InputField XScaleInput;
    public TMP_InputField YScaleInput;
    public TMP_InputField ZScaleInput;

    private GameObject[] measureLines = new GameObject[4];

    private static SelectManager instance;
    public static SelectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SelectManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();

        if (objectNameText == null && GameObject.Find("ObjectNameText").GetComponent<TMP_Text>() != null)
        {
            objectNameText = GameObject.Find("ObjectNameText").GetComponent<TMP_Text>();
        }

        if (XScaleInput == null && GameObject.Find("InputFieldX").GetComponent<TMP_InputField>() != null)
        {
            XScaleInput = GameObject.Find("InputFieldX").GetComponent<TMP_InputField>();
        }
        if (YScaleInput == null && GameObject.Find("InputFieldY").GetComponent<TMP_InputField>() != null)
        {
            YScaleInput = GameObject.Find("InputFieldY").GetComponent<TMP_InputField>();
        }
        if (ZScaleInput == null && GameObject.Find("InputFieldZ").GetComponent<TMP_InputField>() != null)
        {
            ZScaleInput = GameObject.Find("InputFieldZ").GetComponent<TMP_InputField>();
        }
        //Adds listeners to the input field and invokes a method when the value changes.
        XScaleInput.onValueChanged.AddListener(delegate { UpdateScale(); });
        YScaleInput.onValueChanged.AddListener(delegate { UpdateScale(); });
        ZScaleInput.onValueChanged.AddListener(delegate { UpdateScale(); });
        //Validate commas as decimal separators
        XScaleInput.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateComma(addedChar, XScaleInput); };
        YScaleInput.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateComma(addedChar, YScaleInput); };
        ZScaleInput.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateComma(addedChar, ZScaleInput); };
        //Get measureLine objects under SelectManager object
        measureLines[0] = transform.root.Find("measureLine1").gameObject;
        measureLines[1] = transform.root.Find("measureLine2").gameObject;
        measureLines[2] = transform.root.Find("measureLine3").gameObject;
        measureLines[3] = transform.root.Find("measureLine4").gameObject;
    }
    private void Start()
    {
        selectUI.SetActive(false); //Hide panel if not already unactive in scene

        //Turn off outlines by default
        Outline[] outlineScrips = FindObjectsOfType<Outline>();
        foreach (Outline outline in outlineScrips)
        {
            outline.enabled = false;
        }
    }

    void Update()
    {
        //We don't want to select object that are behind UI.
        //IsPointerOverGameObject() == false means mouse is not over UI elements
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.gameObject.layer == 6)
                {
                    //Root object is selected in Select() function if it is not MeasureTool
                    Select(hit.collider.gameObject);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Deselect();
        }
    }

    /// <summary>
    /// Function to set object as selected object, which can then be moved, customized, deleted.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objectHasBeenPlacedBefore">This bool is used to determinate if object was selected from the scene or if it was spawned with buildingManager</param>
    public void Select(GameObject obj)
    {
        if (obj == selectedObject) { return; }
        if (selectedObject != null) { Deselect(); }

        //Outline must be added if selected object doesn't have it!!!
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

        //Hide scale fields when using measure tool
        if (selectedObject.transform.root.tag.Contains("MeasureTool"))
        {
            selectUI.transform.Find("InputFieldX").gameObject.SetActive(false);
            selectUI.transform.Find("InputFieldY").gameObject.SetActive(false);
            selectUI.transform.Find("InputFieldZ").gameObject.SetActive(false);
        }
        else
        {
            //Show correct scale in Input field text fields
            if (selectUI.transform.Find("InputFieldX").GetComponent<TMP_InputField>() != null)
            {
                selectUI.transform.Find("InputFieldX").GetComponent<TMP_InputField>().text = ""; // selectedObject.transform.localScale.x.ToString();
            }
            if (selectUI.transform.Find("InputFieldY").GetComponent<TMP_InputField>() != null)
            {
                selectUI.transform.Find("InputFieldY").GetComponent<TMP_InputField>().text = ""; // selectedObject.transform.localScale.y.ToString();
            }
            if (selectUI.transform.Find("InputFieldZ").GetComponent<TMP_InputField>() != null)
            {
                selectUI.transform.Find("InputFieldZ").GetComponent<TMP_InputField>().text = ""; // selectedObject.transform.localScale.z.ToString();
            }

            selectUI.transform.Find("InputFieldX").gameObject.SetActive(true);
            selectUI.transform.Find("InputFieldY").gameObject.SetActive(true);
            selectUI.transform.Find("InputFieldZ").gameObject.SetActive(true);
        }

        selectUI.SetActive(true);

        selectUI.transform.Find("InputFieldX").GetComponent<TMP_InputField>().transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TMP_Text>().text = selectedObject.transform.localScale.x.ToString();
        selectUI.transform.Find("InputFieldY").GetComponent<TMP_InputField>().transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TMP_Text>().text = selectedObject.transform.localScale.y.ToString();
        selectUI.transform.Find("InputFieldZ").GetComponent<TMP_InputField>().transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TMP_Text>().text = selectedObject.transform.localScale.z.ToString();

    }

    public void Deselect()
    {
        if (selectedObject != null && isMoving == false)
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

            if (selectedObject.transform.root.gameObject.tag != "MeasureTool" && selectedObject.transform.root.GetComponentInChildren<CheckBuildablePlacement>().hasMeasurePoints == true)
            {
                //Hide distance measureLines
                // GameObject[] measurePoints = selectedObject.transform.root.GetComponentInChildren<CheckBuildablePlacement>().measurePoints;
                foreach (GameObject obj in measureLines)
                {
                    obj.SetActive(false);
                }
            }
            //Clear input fields so that next selected object is not affected by last input scales
            XScaleInput.text = "";
            YScaleInput.text = "";
            ZScaleInput.text = "";

            //Place to first position if moving is still happening
         /*   if (buildingManager.pendingObject != null)
            {
                //Placing object in buildingManager makes pendingObject null.
                buildingManager.PlaceObject();
            } */


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
        if (selectedObject != null && selectedObject.transform.root.tag != "MeasureTool")
        {
            //Either the parent or a child has to have CheckBuildablePlacement script
            if (selectedObject.GetComponent<CheckBuildablePlacement>() != null)
            {
                if (selectedObject.GetComponent<CheckBuildablePlacement>().hasMeasurePoints == true)
                {
                    UpdateMeasureLines();
                }
            }
            else if (selectedObject.transform.root.GetComponentInChildren<CheckBuildablePlacement>() != null)
            {
                if (selectedObject.GetComponentInChildren<CheckBuildablePlacement>().hasMeasurePoints == true)
                {
                    UpdateMeasureLines();
                }
            }
        }
    }

    void UpdateMeasureLines()
    {
        GameObject[] measurePoints = selectedObject.transform.root.GetComponentInChildren<CheckBuildablePlacement>().measurePoints;

        //4 transform where rays are shot outwards to show distance to closest object
        //measure points must face away from object because we use transform.forward
        MeasureDistance(measurePoints[0].transform, measureLines[0]);
        MeasureDistance(measurePoints[1].transform, measureLines[1]);
        MeasureDistance(measurePoints[2].transform, measureLines[2]);
        MeasureDistance(measurePoints[3].transform, measureLines[3]);
    }

    /// <summary>
    /// Origin is measurePoint position and rayObj is the measureLine object with line renderer and text UI
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="rayObj"></param>
    void MeasureDistance(Transform origin, GameObject rayObj)
    {
        selectUI.transform.Find("InputFieldX").GetComponent<TMP_InputField>().transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TMP_Text>().text = selectedObject.transform.localScale.x.ToString();
        selectUI.transform.Find("InputFieldY").GetComponent<TMP_InputField>().transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TMP_Text>().text = selectedObject.transform.localScale.y.ToString();
        selectUI.transform.Find("InputFieldZ").GetComponent<TMP_InputField>().transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TMP_Text>().text = selectedObject.transform.localScale.z.ToString();

        rayObj.SetActive(true);
        rayObj.transform.position = origin.position;
        rayObj.transform.rotation = origin.rotation;
        Ray distanceRay = new Ray(origin.position, origin.forward);
        RaycastHit hit;

        //Update distance text and visualize path with Line Renderer in all measureLines
        if (Physics.Raycast(distanceRay, out hit))
        {
            //.ToString(".0####") amount of # means how many decimals show in float
            rayObj.GetComponentInChildren<TextMeshPro>().text = hit.distance.ToString(".0#") + "m";
            rayObj.GetComponent<LineRenderer>().SetPosition(0, origin.position);
            rayObj.GetComponent<LineRenderer>().SetPosition(1, hit.point);
        }
        // Rotate distance text so that it is towards camera side.
        if (rayObj.transform.Find("distanceText") != null)
        {
            GameObject distanceText = rayObj.transform.Find("distanceText").gameObject;
            //Rotate towards measurePoint and align rotation
            distanceText.transform.LookAt(origin.position);
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
            else
            {
                eulerAngles.y += 90;
            }
            //Don't rotate on x,z axis
            eulerAngles.x = 0;
            eulerAngles.z = 0;
            distanceText.transform.rotation = Quaternion.Euler(eulerAngles);
        }
    }

    public void UpdateScale()
    {
        float xScale = selectedObject.transform.localScale.x;
        float yScale = selectedObject.transform.localScale.y;
        float zScale = selectedObject.transform.localScale.z;

        bool isXNum = float.TryParse(XScaleInput.text, out xScale);
        bool isYNum = float.TryParse(YScaleInput.text, out yScale);
        bool isZNum = float.TryParse(ZScaleInput.text, out zScale);

        if (isXNum && xScale > 0)
        {
            selectedObject.transform.localScale = new Vector3(xScale, selectedObject.transform.localScale.y, selectedObject.transform.localScale.z);
        }
        if (isYNum && yScale > 0)
        {
            selectedObject.transform.localScale = new Vector3(selectedObject.transform.localScale.x, yScale, selectedObject.transform.localScale.z);
        }
        if (isZNum && zScale > 0)
        {
            selectedObject.transform.localScale = new Vector3(selectedObject.transform.localScale.x, selectedObject.transform.localScale.y, zScale);
        }

    }

    /// <summary>
    /// Unity uses dot as separator, but user might use comma. This turns commas into dots.
    /// </summary>
    /// <param name="charToValidate"></param>
    /// <param name="inputField">This looks like obsolete parameter</param>
    /// <returns></returns>
    private char ValidateComma(char charToValidate, TMP_InputField inputField)
    {
        //Now input fields accept commas as decimal separators
        if (charToValidate == ',')
        {
            charToValidate = '.';
        }
        return charToValidate;
    }

}
