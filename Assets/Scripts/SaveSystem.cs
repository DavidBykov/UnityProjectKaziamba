using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static void SaveLevelStatucByID(string LevelID, bool status)
    {
        if (status == true)
            PlayerPrefs.SetInt(LevelID, 1);
        else
            PlayerPrefs.SetInt(LevelID, 0);
    }

    public static bool LoadLevelStatucByID(string LevelID)
    {
        if (PlayerPrefs.GetInt(LevelID) == 1)
            return true;
        else
            return false;
    }
}
