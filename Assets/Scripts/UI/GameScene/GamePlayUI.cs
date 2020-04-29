using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayUI : MonoBehaviour
{
    public GameObject timerUI;
    public GameObject soulsUI;
    public GameEndUI gameEndUI;
    private GameSettings gamePlaySettings;

    void Start()
    {
        gamePlaySettings = FindObjectOfType<GameSettings>();

        if(gamePlaySettings.GetGameParemeters().gameTime == -1)
        {
            timerUI.SetActive(false);
        }

        if(gamePlaySettings.GetGameParemeters().neededEnergy == -1)
        {
            soulsUI.SetActive(false);
        }
    }
}
