using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEditor;
using System.Text;
using Palmmedia.ReportGenerator.Core.Common;
using UnityEngine.SceneManagement;

//GameObject's name and Transform values must be saved.
//Objects are loaded with Resources.Load using their name, and then given correct Transform values.
//All Prefabs that are used for loading resources must be placed in Assets/Resources/Prefabs folder!!!
public class SaveGameManager : MonoBehaviour
{

    private static SaveGameManager instance;
    public static SaveGameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<SaveGameManager>();
            }
            return instance;
        }
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

    public void SaveBinary()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/testSave1.save";
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
            data.posX = rootObj.transform.position.x;
            data.posY = rootObj.transform.position.y;
            data.posZ = rootObj.transform.position.z;
            data.rotX = rootObj.transform.rotation.x;
            data.rotY = rootObj.transform.rotation.y;
            data.rotZ = rootObj.transform.rotation.z;
            data.scaleX = rootObj.transform.localScale.x;
            data.scaleY = rootObj.transform.localScale.z;
            data.scaleZ = rootObj.transform.localScale.x;

            allData.saveDatas[i] = data;
            i++;     
        }
        formatter.Serialize(stream, allData);
        stream.Close();
    }

    public void LoadBinary()
    {
        string path = Application.persistentDataPath + "/testSave1.save";
        if (File.Exists(path))
        {
            //DELETE ALL BUILDABLES FROM SCENE SO THAT LOADED ONES CAN REPLACE THEM
            //CheckBuildablePlacement[] saveObjects = FindObjectsOfType<CheckBuildablePlacement>();
            GameObject[] buildables = GameObject.FindGameObjectsWithTag("Buildable");
            foreach (GameObject obj in buildables)
            {
                Destroy(obj.transform.root.gameObject);
            }

            //GET SAVE DATA 
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            AllData allData = formatter.Deserialize(stream) as AllData;

            foreach (SaveData saveData in allData.saveDatas)
            {
                //INSTANTIATE GAMEOBJECT'S NAME AND TRANSFORM WITH DATA
                GameObject spawnObj = Instantiate(Resources.Load("Prefabs/" + saveData.name) as GameObject);
                spawnObj.name = Resources.Load("Prefabs/" + saveData.name).name;
                spawnObj.transform.position = new Vector3(saveData.posX, saveData.posY, saveData.posZ);
                //Set Outlines inactive
                if (spawnObj.GetComponent<Outline>() != null) { spawnObj.GetComponent<Outline>().enabled = false; }
                if (spawnObj.GetComponentInChildren<Outline>() != null) { spawnObj.GetComponentInChildren<Outline>().enabled = false; }
            }
            stream.Close();      
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
        }
    }

}

[System.Serializable]
public class SaveData
{
    public string name;
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
}