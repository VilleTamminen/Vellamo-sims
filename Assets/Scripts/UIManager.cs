using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button MenuPanelButton;
    public Button ObjectPanelButton;
    public Button QuitButton;

    public GameObject MenuPanel;
    public GameObject ObjectPanel;
    public GameObject QuitPanel;

    // Start is called before the first frame update
    void Start()
    {
        if(MenuPanelButton == null)
        {
            MenuPanelButton = GameObject.Find("MenuPanelButton").GetComponent<Button>();
        }
        if (MenuPanel == null)
        {
            MenuPanel = GameObject.Find("MenuPanel");
            MenuPanel.SetActive(false);
        }
        if (ObjectPanelButton == null)
        {
            ObjectPanelButton = GameObject.Find("ObjectPanelButton").GetComponent<Button>();
        }
        if (ObjectPanel == null)
        {
            ObjectPanel = GameObject.Find("ObjectPanel");
        }
        if (QuitButton == null)
        {
            QuitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        }
        if (QuitPanel == null)
        {
            QuitPanel = GameObject.Find("QuitPanel");
            QuitPanel.SetActive(false);
        }

        MenuPanelButton.onClick.AddListener(ToggleMenuPanelView);
        ObjectPanelButton.onClick.AddListener(ToggleObjectPanelView);
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
    public void QuitGame()
    {
        Application.Quit();
    }

}
