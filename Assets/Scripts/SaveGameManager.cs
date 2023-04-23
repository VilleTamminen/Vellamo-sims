using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using System.Runtime.Serialization;
using System.IO.Pipes;

//SAVE SYSTEM. GameObject's name and Transform values must be saved.
//Objects are loaded with Resources.Load using their name, and then given correct Transform values.
//All Prefabs that are used for loading resources must be placed in Assets/Resources/Prefabs folder!!!
//Save game names and file type must not be changed in Saves folder. save1.save, save2.save etc...
//2 functions to update save game buttons content. 1st has predeterminated amount of save game buttons, that stay. 2nd function adds and deletes buttons as needed (HAS BUGS!!!).
public class SaveGameManager : MonoBehaviour
{
    private string saveGameStringId = ""; //string id is used to delete file from Saves folder
    private int saveGameIntId = 0; //int id is used to destroy save game button from UI menu with saveGameButtons List
    public int saveGameCount = 0;
    private static SaveGameManager instance;

    public List<GameObject> saveGameButtons;
    public GameObject saveGameButtonPrefab; //Needed if UpdateSaveGameContent() function is used.
    public GameObject saveGameButtonParent; //Content GameObject in Scroll View

    public static SaveGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SaveGameManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        foreach (Transform child in saveGameButtonParent.transform)
        {
            saveGameButtons.Add(child.gameObject);
        }

        int i = 0;
        foreach (GameObject child in saveGameButtons)
        {
            //must use local copy, so that value changes are kept in every loop (otherwise last value gets used in every AddListener)!!!
            int localCopyOfSaveGameCount = i;
            GameObject newSaveGameButton = saveGameButtons[localCopyOfSaveGameCount];
            newSaveGameButton.GetComponent<Button>().onClick.RemoveAllListeners();
            // string fileName = "save" + i.ToString() + ".save";
            string fileName = saveGameButtons[localCopyOfSaveGameCount].GetComponent<SaveGameButton>().saveGameStringId;
            newSaveGameButton.GetComponent<Button>().onClick.AddListener(() => SelectSaveGame(fileName, localCopyOfSaveGameCount));
            i++;
        }
        UpdateSaveGameButtons();
    }
    /*
    public void SaveJson()
    {
        string path = Application.persistentDataPath + "/testSave2.json";
        FileStream stream = new FileStream(path, FileMode.Create);

        //COLLECT SAVE DATA
        //Right now CheckBuildablePlacement is the only script that every buildable object has, so it is used to find all buildables.
        CheckBuildablePlacement[] saveObjects = FindObjectsOfType<CheckBuildablePlacement>();
        List<GameObject> saveObjectRoots = new List<GameObject>();

        var sb = new StringBuilder();
        foreach (CheckBuildablePlacement obj in saveObjects)
        {
            Debug.Log(obj.transform.root.gameObject.name);
            GameObject rootObj = obj.transform.root.gameObject;
            SaveData data = new SaveData();

            data.name = rootObj.name;
            data.posX = rootObj.transform.position.x;
            data.posY = rootObj.transform.position.y;
            data.posZ = rootObj.transform.position.z;
            data.rotX = rootObj.transform.rotation.x;
            data.rotY = rootObj.transform.rotation.y;
            data.rotZ = rootObj.transform.rotation.z;
            data.scaleX = rootObj.transform.localScale.x;
            data.scaleY = rootObj.transform.localScale.z;
            data.scaleZ = rootObj.transform.localScale.x;

            string json = JsonUtility.ToJson(data, true);
            Debug.Log("json: " + json.ToString());
            sb.AppendLine(json);
        }
        using (StreamWriter writer = new StreamWriter(stream))
        {
            writer.Write(sb);
        }
        stream.Close();    
    } */

    public void SaveBinary(string saveFile)
    {
        //Check if Saves folder exists
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Saves/" + saveFile;
        FileStream stream = new FileStream(path, FileMode.Create);

        //COLLECT SAVE DATA
        //Right now CheckBuildablePlacement is the only script that every buildable object has, so it is used to find all buildables.
        CheckBuildablePlacement[] saveObjects = FindObjectsOfType<CheckBuildablePlacement>();

        AllData allData = new AllData();
        allData.saveDatas = new SaveData[saveObjects.Length];
        int i = 0;
        foreach (CheckBuildablePlacement obj in saveObjects)
        {
            GameObject rootObj = obj.transform.root.gameObject;
            SaveData data = new SaveData();

            data.name = rootObj.name;
            //  data.displayName = rootObj.displayName;
            data.posX = rootObj.transform.position.x;
            data.posY = rootObj.transform.position.y;
            data.posZ = rootObj.transform.position.z;
            //Must use eulerAngles, otherwise rotation saving fails!!!
            data.rotX = rootObj.transform.eulerAngles.x;
            data.rotY = rootObj.transform.eulerAngles.y;
            data.rotZ = rootObj.transform.eulerAngles.z;
            //Must use float inbetween setting scale values, otherwise scale saving fails!!!
            float scaleX = rootObj.transform.localScale.x;
            float scaleY = rootObj.transform.localScale.y;
            float scaleZ = rootObj.transform.localScale.z;
            data.scaleX = scaleX;
            data.scaleY = scaleY;
            data.scaleZ = scaleZ;

            allData.saveDatas[i] = data;
            i++;
        }
        //File gets overwritten or created with data
        formatter.Serialize(stream, allData);
        stream.Close();
    }

    public void LoadBinary(string loadFile)
    {
        // string path = Application.persistentDataPath + "/testSave1.save";
        string path = Application.persistentDataPath + "/Saves/" + loadFile;

        if (File.Exists(path))
        {
            //DELETE ALL BUILDABLES FROM SCENE SO THAT LOADED ONES CAN REPLACE THEM
            //Deleting with CheckBuildablePlacement deletes MeasureTools, deleting with Buildable tag does not.
            //CheckBuildablePlacement[] buildables = FindObjectsOfType<CheckBuildablePlacement>();
            List<GameObject> buildables = new List<GameObject>(GameObject.FindGameObjectsWithTag("Buildable"));
            buildables.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("MeasureTool")));
            foreach (GameObject obj in buildables)
            {
                Destroy(obj.transform.root.gameObject);
            }

            //GET SAVE DATA 
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            //Don't deserialize empty streams
            if (stream.Length > 0)
            {
                AllData allData = formatter.Deserialize(stream) as AllData;
                foreach (SaveData saveData in allData.saveDatas)
                {
                    //Check if prefab exists
                    if (Resources.Load("Prefabs/" + saveData.name))
                    {
                        //INSTANTIATE GAMEOBJECT'S NAME AND TRANSFORM WITH DATA
                        GameObject spawnObj = Instantiate(Resources.Load("Prefabs/" + saveData.name) as GameObject);
                        spawnObj.name = Resources.Load("Prefabs/" + saveData.name).name;
                        spawnObj.transform.position = new Vector3(saveData.posX, saveData.posY, saveData.posZ);
                        Vector3 eulerAngles = new Vector3(saveData.rotX, saveData.rotY, saveData.rotZ);
                        spawnObj.transform.localRotation = Quaternion.Euler(eulerAngles);
                        Vector3 newScale = new Vector3(saveData.scaleX, saveData.scaleY, saveData.scaleZ);
                        spawnObj.transform.localScale = newScale;

                        //Set Outlines inactive
                        if (spawnObj.GetComponent<Outline>() != null) { spawnObj.GetComponent<Outline>().enabled = false; }
                        if (spawnObj.GetComponentInChildren<Outline>() != null) { spawnObj.GetComponentInChildren<Outline>().enabled = false; }
                    }
                    else
                    {
                        Debug.LogError("Prefab resource asset does not exists with name " + saveData.name);
                    }
                }
            }
            stream.Close();
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
        }
    }
    public void DeleteSave()
    {
        //Deletes currently selected save with saveGameId
        string path = Application.persistentDataPath + "/Saves/" + saveGameStringId;

        if (!File.Exists(path))
        {
            Debug.Log("no save file exists");
        }
        else
        {
            Debug.Log("save file exists, deleting...");
            File.Delete(path); //deletes the file
            //File.WriteAllText(path, ""); //empties the file

            //Delete save game button
            // Destroy(saveGameButtons[saveGameIntId]);
            /*   if (GameObject.Find("SaveGame" + saveGameIntId.ToString()).GetComponentInChildren<TMP_Text>() != null)
               {
                   GameObject.Find("SaveGame" + saveGameIntId.ToString()).GetComponentInChildren<TMP_Text>().text = "deleted";
               }  */
        }
        //UpdateSaveGameContent();
        UpdateSaveGameButtons();
    }

    /// <summary>
    /// SaveGameIcon buttons in MenuPanel use this: SaveGame1 uses 1, SaveGame2 uses 2 etc...
    /// </summary>
    /// <param name="id"></param>
    public void SelectSaveGame(string stringId, int intId)
    {
        //When save game button is pressed, it gets selected, which allows saving/loading from it.
        saveGameStringId = stringId;
        saveGameIntId = intId;
        Debug.Log("selected save game string id: " + saveGameStringId + " , and int id: " + saveGameIntId);

        //Highlight button image color and turn other button imnages to normal
        foreach(GameObject child in saveGameButtons)
        {           
            if(child.GetComponent<SaveGameButton>().saveGameStringId == saveGameStringId)
            {
                //Highlight selected button image with light green color
                child.GetComponent<Image>().color = new Color32(180, 255, 165, 255);
            }
            else
            {
                //plain white for other buttons
                child.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }
    public void SaveGame()
    {
        // string saveFile = "save" + saveGameId.ToString() + ".save";
        SaveBinary(saveGameStringId);
        // UpdateSaveGameContent();
        UpdateSaveGameButtons();
    }
    public void LoadGame()
    {
        //Deselect any selected objects
        SelectManager.Instance.Deselect();

        // string loadFile = "save" + saveGameId.ToString() + ".save";
        //  LoadBinary(saveGameStringId);
        LoadBinary(saveGameButtons[saveGameIntId].GetComponent<SaveGameButton>().saveGameStringId);
        // UpdateSaveGameContent();
        UpdateSaveGameButtons();
    }
    public void NewGame()
    {
        //Add new game 
        GameObject newSaveGameButton = Instantiate(saveGameButtonPrefab, saveGameButtonParent.transform);
        newSaveGameButton.GetComponent<Button>().onClick.RemoveAllListeners();
        string newSaveName = "save" + (saveGameCount + 1).ToString() + ".save";
        newSaveGameButton.GetComponent<Button>().onClick.AddListener(() => SelectSaveGame(newSaveName, saveGameCount + 1));
        newSaveGameButton.GetComponentInChildren<TMP_Text>().text = "+";
        saveGameButtons.Add(newSaveGameButton);
    }

    private void UpdateSaveGameCount()
    {
        //not used for now!!!
        saveGameCount = 0;
        var fileInfo = new DirectoryInfo(Application.persistentDataPath + "/Saves").GetFiles();
        foreach (FileInfo file in fileInfo)
        {
            if (file.Extension == ".save")
            {
                saveGameCount++;
            }
        }
    }

    /// <summary>
    /// Uses already existing save game buttons, does not create new ones. Limited and predetermined amount of saves.
    /// </summary>
    public void UpdateSaveGameButtons()
    {
        //Go through all saveGameButtons and if there is a save file match, add file name and timestamp to button text       
        var fileInfo = new DirectoryInfo(Application.persistentDataPath + "/Saves").GetFiles();
        foreach (GameObject saveGameButton in saveGameButtons)
        {
            bool saveFileMatch = false;
            foreach (FileInfo file in fileInfo)
            {
                if (file.Name == saveGameButton.GetComponent<SaveGameButton>().saveGameStringId)
                {
                    saveFileMatch = true;
                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream stream = new FileStream(file.FullName, FileMode.Open);
                    //Don't deserialize empty stream
                    if (stream.Length == 0)
                    {
                        //Don't open empty files, just put + on the text field
                        saveGameButton.GetComponentInChildren<TMP_Text>().text = "+";
                    }
                    else
                    {
                        AllData allData = formatter.Deserialize(stream) as AllData;
                        saveGameButton.GetComponentInChildren<TMP_Text>().text = file.Name + "\n" + allData.timestamp.ToString("dd/MM/yyyy");
                    }
                    stream.Close();
                }
            }
            if (saveFileMatch == false)
            {
                saveGameButton.GetComponentInChildren<TMP_Text>().text = "+";
            }
        }
    }

    /// <summary>
    /// Instantiates save game buttons
    /// </summary>
    public void UpdateSaveGameContent()
    {
        //Delete save game buttons
        foreach (Transform child in saveGameButtonParent.transform)
        {
            Destroy(child.gameObject);
        }
        saveGameButtons.Clear();

        //Create new save game buttons and show their name and save date
        var fileInfo = new DirectoryInfo(Application.persistentDataPath + "/Saves").GetFiles();
        int saveGameCount = 0;
        GameObject newSaveGameButton;
        foreach (FileInfo file in fileInfo)
        {
            if (file.Extension == ".save")
            {
                //must use local copy, so that value changes are kept in every loop (otherwise last value gets used in every AddListener)!!!
                int localCopyOfSaveGameCount = saveGameCount;

                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(file.FullName, FileMode.Open);
                AllData allData = formatter.Deserialize(stream) as AllData;
                if (allData != null)
                {
                    newSaveGameButton = Instantiate(saveGameButtonPrefab, saveGameButtonParent.transform);
                    newSaveGameButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    newSaveGameButton.GetComponent<Button>().onClick.AddListener(() => SelectSaveGame(file.Name, localCopyOfSaveGameCount));
                    newSaveGameButton.GetComponentInChildren<TMP_Text>().text = file.Name + "\n" + allData.timestamp.ToString("dd/MM/yyyy");
                    saveGameButtons.Add(newSaveGameButton);
                }

                Debug.Log("save game count: " + saveGameCount);
                stream.Close();
                saveGameCount++;
            }
        }

    }
}

[System.Serializable]
public class SaveData
{
    public string name;
    public string displayName; //Since this game uses Finnish or English, or both, Finnish display name should be saved. Prefabs might use English.
    public float posX;
    public float posY;
    public float posZ;
    public float rotX;
    public float rotY;
    public float rotZ;
    public float scaleX;
    public float scaleY;
    public float scaleZ;
}

[System.Serializable]
class AllData
{
    //Needed so that all saveData entries can be saved or loaded in one go.
    public SaveData[] saveDatas;
    public DateTime timestamp = System.DateTime.Now;
}