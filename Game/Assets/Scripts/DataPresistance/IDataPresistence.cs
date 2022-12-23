using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPresistence //if you want control saveload stuff from UI - thats the plase you need
{
    void LoadData(GameData data);

    void SaveData(ref GameData data);
}
