using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button MenuPanelButton;
    public Button ObjectPanelButtonOpen;
    public Button ObjectPanelButtonClose;
    public Button QuitButton;
    public Button ControlsPanelButtenOpen;
    public Button ControlsPanelButtenClose;

    public GameObject MenuPanel;
    public GameObject ObjectPanel;
    public GameObject QuitPanel;
    public GameObject ControlsPanel;

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

    public void QuitGame()
    {
        Application.Quit();
    }

}
