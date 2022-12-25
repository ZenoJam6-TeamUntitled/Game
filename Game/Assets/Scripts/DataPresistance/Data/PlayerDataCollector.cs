using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataCollector : MonoBehaviour, IDataPresistence
{

    int currentLevelIndex;

    public void LoadData(GameData data)
    {
        this.currentLevelIndex = data.currentLevelIndex;
        this.transform.position = data.playerPosition;
    }

    public void SaveData(ref GameData data)
    {
        data.currentLevelIndex = this.currentLevelIndex;
        data.playerPosition = this.transform.position;
    }


    private void GetCurrentLevelIndex()
    //Getting level index
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

}
