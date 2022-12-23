using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPresistanceManager : MonoBehaviour
{

    public static DataPresistanceManager instance { get; private set; }

    private GameData gameData;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Presistance Manager in the scene");
        }
        instance = this;
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // TODO - Load saved data


        //if no saved data starting new
        if (this.gameData == null)
        {
            Debug.Log("No data has been found. Initialising defaults.");                
            NewGame();
            
        }

        // TODO - Push loaded data for other scripts

    }

    public void SaveGame()
    {
        // TODO - pass data to other scripts to update

        // TODO - save data to a file using data handler

    }

}
