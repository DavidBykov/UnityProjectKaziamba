using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public LoadingSceneUI loadingSceneUI;
    public LevelDescriptionUI levelDescriptionUI;
    public Text buttonText;

    private bool sceneFounded;

    private void OnEnable()
    {
        GameEconomy.curentItem = null;
        List<LevelConfiguration> levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList();
        if (levelConfigurations.First().completed == false)
        {
            buttonText.text = "НОВАЯ ИГРА";
        } else
        {
            buttonText.text = "ПРОДОЛЖИТЬ";
        }
    } 

    public void ContinueGame()
    {
        List<LevelConfiguration> levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList();
        for (int i = 0; i < levelConfigurations.Count; i++) 
        {
            if(levelConfigurations[i].completed == false || i == levelConfigurations.Count - 1)
            {
                if (levelConfigurations[i].startWithoutDescription)
                {
                    loadingSceneUI.SetLoadingConfiguration(levelConfigurations[i]);
                    loadingSceneUI.gameObject.SetActive(true);
                } else
                {
                    levelDescriptionUI.SetLevelConfiguration(levelConfigurations[i]);
                    levelDescriptionUI.gameObject.SetActive(true);
                }

                GameEconomy.curentLevel = levelConfigurations[i];
                break;
            }
        }


    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
