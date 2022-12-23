using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData : MonoBehaviour
{
    public int currentLevelIndex;

    //initial data values to start game
    public GameData()
        {
        this.currentLevelIndex = 0;
        }

}
