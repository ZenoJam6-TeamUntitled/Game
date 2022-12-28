using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPresistenceManager : MonoBehaviour
{
    public static DataPresistenceManager instance { get; private set; }

    private GameData gameData;

    private List<IDataPresistence> dataPresistenceObjects;

    private FileDataHandler dataHandler;

    private float saveTimeLoop;
    //private bool checkpointActive = true; //variable for controlling checkpoint function work
    [Header("File Storage Config")]

    [SerializeField] private bool timeCheckpoints = true;
    [SerializeField] private string fileName;
    [SerializeField] private float timeToWait; //Time between checkpoints




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
        CheckpointTimeHadler();

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPresistenceObjects = FindAllDataPresistenceObjects();
        LoadGame(); //can be removed 
    }
    private void Update()
    {
        CheckpointSave();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()  // TODO - rework, when IU done
    {
        // Load saved data
        this.gameData = dataHandler.Load();

        // if no saved data starting new game
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
            foreach (IDataPresistence dataPresistenceObj in dataPresistenceObjects)
            {
                dataPresistenceObj.SaveData(ref gameData);
            }

            dataHandler.Save(gameData);
    }

    private void CheckpointTimeHadler()
    {
        if (timeToWait < 10)
        {
            timeCheckpoints = false;
        }
        else
        {
            saveTimeLoop = timeToWait;
        }
    }

    public void CheckpointSave()
    {
        if (timeCheckpoints)
        {
            if (Time.time > saveTimeLoop)
            {
                SaveGame();
                saveTimeLoop += timeToWait;
            }
        }
    }

    private List<IDataPresistence> FindAllDataPresistenceObjects()
    {

        IEnumerable<IDataPresistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPresistence>();

    
        return new List<IDataPresistence>(dataPersistenceObjects);
    }
}
