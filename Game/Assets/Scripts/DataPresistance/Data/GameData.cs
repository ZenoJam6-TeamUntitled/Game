using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData : MonoBehaviour
{
    int levelNumber;

    //initial data values to start game
    public GameData()
        {
        this.levelNumber = 1;
        }

}
