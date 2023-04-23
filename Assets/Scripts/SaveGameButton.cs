using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameButton : MonoBehaviour
{
    //Needed to store a reference to save file name
    public string saveGameStringId = "";

    /// <summary>
    /// Not used currently.
    /// </summary>
    public void SetSaveGameStringId(string id)
    {
        saveGameStringId = id;
    }
}
