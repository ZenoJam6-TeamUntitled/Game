using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPresistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    public static DataPresistenceManager instance { get; private set; }

    private GameData gameData;

    private List<IDataPresistence> dataPresistenceObjects;

    private FileDataHandler dataHandler;


    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Presistence Manager in the scene");
        }
        instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPresistenceObjects = FindAllDataPresistenceObjects();       
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

        //Push loaded data for other scripts
        foreach(IDataPresistence dataPresistenceObj in dataPresistenceObjects)
        {
            dataPresistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        // Pass data to other scripts to update
        foreach (IDataPresistence dataPresistenceObj in dataPresistenceObjects)
        {
            dataPresistenceObj.LoadData(gameData);
        }

        // TODO - save data to a file using data handler

    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPresistence> FindAllDataPresistenceObjects()
    {

        IEnumerable<IDataPresistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPresistence>();

    
        return new List<IDataPresistence>(dataPersistenceObjects);
    }
}
