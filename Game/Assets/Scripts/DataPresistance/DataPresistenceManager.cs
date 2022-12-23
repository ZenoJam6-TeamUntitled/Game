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

    [SerializeField] private float timeToWait;
    private float saveTimeLoop;


    
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
        saveTimeLoop = timeToWait;
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPresistenceObjects = FindAllDataPresistenceObjects();
        LoadGame();
    }
    private void Update()
    {
        if (Time.time > timeToWait)
        {
            Debug.Log("Time is working");
            SaveGame();
            timeToWait += saveTimeLoop;
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load saved data
        this.gameData = dataHandler.Load();

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
       
        foreach (IDataPresistence dataPresistenceObj in dataPresistenceObjects)
        {
            dataPresistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);

    }

    private void ButtonSave()
    {
        if (Input.GetKeyDown(KeyCode.P))
            {

            SaveGame();
            Debug.Log("Keycode working");
            }
    }

    private List<IDataPresistence> FindAllDataPresistenceObjects()
    {

        IEnumerable<IDataPresistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPresistence>();

    
        return new List<IDataPresistence>(dataPersistenceObjects);
    }
}
