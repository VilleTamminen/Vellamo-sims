using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Button MenuPanelButton;
    public Button ObjectPanelButtonOpen;
    public Button ObjectPanelButtonClose;
    public Button QuitButton;
    public Button ControlsPanelButtenOpen;
    public Button ControlsPanelButtenClose;
   // public Toggle BlueprintToggle; 

    public GameObject MenuPanel;
    public GameObject ObjectPanel;
    public GameObject QuitPanel;
    public GameObject ControlsPanel;
    public GameObject FloorBlueprint;

    public List<GameObject> ObjectButtons;
    public GameObject ContentParent; //Content of object panel object buttons
    public GameObject SearchBar;


    private void Awake()
    {
        FillObjectPanelButtonsInfo();
    }
    // Start is called before the first frame update
    void Start()
    {
        //Menu
        if(MenuPanelButton == null)
        {
            MenuPanelButton = GameObject.Find("MenuPanelButton").GetComponent<Button>();
        }
        if (MenuPanel == null)
        {
            MenuPanel = GameObject.Find("MenuPanel");
            MenuPanel.SetActive(false);
        }
        //Object panel
        if (ObjectPanelButtonOpen == null)
        {
            ObjectPanelButtonOpen = GameObject.Find("ObjectPanelButtonOpen").GetComponent<Button>();
        }
        if (ObjectPanelButtonClose == null)
        {
            ObjectPanelButtonClose = GameObject.Find("ObjectPanelButtonClose").GetComponent<Button>();
        }
        if (ObjectPanel == null)
        {
            ObjectPanel = GameObject.Find("ObjectPanel");
        }
        //Quit
        if (QuitButton == null)
        {
            QuitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        }
        if (QuitPanel == null)
        {
            QuitPanel = GameObject.Find("QuitPanel");
            QuitPanel.SetActive(false);
        }
        //Controls
        if (ControlsPanelButtenOpen == null)
        {
            ControlsPanelButtenOpen = GameObject.Find("ControlsPanelButtenOpen").GetComponent<Button>();
        }
        if (ControlsPanelButtenClose == null)
        {
            ControlsPanelButtenClose = GameObject.Find("ControlsPanelButtenClose").GetComponent<Button>();
        }
        if (ControlsPanel == null)
        {
            ControlsPanel = GameObject.Find("ControlsPanel");
            ControlsPanel.SetActive(false);
        }

        MenuPanelButton.onClick.AddListener(ToggleMenuPanelView);
        ObjectPanelButtonOpen.onClick.AddListener(ToggleObjectPanelView);
        ObjectPanelButtonClose.onClick.AddListener(ToggleObjectPanelView);
        ControlsPanelButtenOpen.onClick.AddListener(ToggleControlsPanelView);
        ControlsPanelButtenClose.onClick.AddListener(ToggleControlsPanelView);
        QuitButton.onClick.AddListener(ToggleQuitPanelView);
    }

    private void ToggleMenuPanelView()
    {
        if (MenuPanel.activeSelf == true)
        {
            MenuPanel.SetActive(false);
        }
        else
        {
            MenuPanel.SetActive(true);
           // SaveGameManager.Instance.UpdateSaveGameContent();
            SaveGameManager.Instance.UpdateSaveGameButtons();
        }
    }
    private void ToggleObjectPanelView()
    {
        if (ObjectPanel.activeSelf == true)
        {
            ObjectPanel.SetActive(false);
        }
        else
        {
            ObjectPanel.SetActive(true);
        }
    }
    public void ToggleQuitPanelView()
    {
        if (QuitPanel.activeSelf == true)
        {
            QuitPanel.SetActive(false);
        }
        else
        {
            QuitPanel.SetActive(true);
        }
    }
    private void ToggleControlsPanelView()
    {
        if (ControlsPanel.activeSelf == true)
        {
            ControlsPanel.SetActive(false);
        }
        else
        {
            ControlsPanel.SetActive(true);
        }
    }
    public void ToggleBlueprint()
    {
        //Show blueprint for floor plan
        if (FloorBlueprint.activeSelf == true)
        {
            FloorBlueprint.SetActive(false);
        }
        else
        {
            FloorBlueprint.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Fill object buttons with corresponding names from BuildingManager objects array. Also sets Button onclick event to select object from buildingmanager.
    /// </summary>
    private void FillObjectPanelButtonsInfo()
    {
        //Get child buttons
        foreach(Transform child in ContentParent.transform)
        {
            ObjectButtons.Add(child.gameObject);
        }
        //Assign button info
        for (int i = 0; i < BuildingManager.Instance.objects.Length - 1; i++)
        {
            //Use copy of i so that it uses correct number and not the last value of i
            int copy = i;
            ObjectButtons[i].GetComponentInChildren<TMP_Text>().text = BuildingManager.Instance.objects[i].name;
            ObjectButtons[i].GetComponentInChildren<TMP_Text>().fontSize = 22;
            ObjectButtons[i].GetComponent<Button>().onClick.AddListener(delegate
            {
                BuildingManager.Instance.SelectObject(copy);
            });
        }
        //Hide unnecessary object buttons
        if (ObjectButtons.Count > BuildingManager.Instance.objects.Length) {
            for (int i = ObjectButtons.Count - 1; i >= BuildingManager.Instance.objects.Length; i--)
            {
                ObjectButtons[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Search object names in Object Panel by seeing if they contain right word. 
    /// </summary>
    /// <param name="prompt"></param>
    public void SearchByName()
    {
        string searchText = SearchBar.GetComponent<TMP_InputField>().text;

        foreach (GameObject child in ObjectButtons)
        {
            if (child.GetComponentInChildren<TMP_Text>().text.Length >= searchText.Length)
            {
                if (child.GetComponentInChildren<TMP_Text>().text.ToLower().Contains(searchText))
                {
                    child.SetActive(true);
                }
                else
                {
                    child.SetActive(false);
                }
            }
        }
    }
}
