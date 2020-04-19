using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenuUI : MonoBehaviour
{
    private bool sceneFounded;

    private void Awake()
    {
        GameEconomy.curentItem = null;
    }

    public void ContinueGame()
    {
        List<LevelConfiguration> levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList();
        for (int i = 0; i < levelConfigurations.Count; i++) 
        {
            if(levelConfigurations[i].completed == false)
            {
                if (i == 0)
                {
                    GameEconomy.curentLevel = levelConfigurations[i];
                    SceneManager.LoadScene(levelConfigurations[i].LoadingScene);
                    
                    sceneFounded = true;
                    break;
                } else
                {
                    GameEconomy.curentLevel = levelConfigurations[i];
                    SceneManager.LoadScene(levelConfigurations[i].LoadingScene);
                    
                    sceneFounded = true;
                    break;
                }
            }
        }
        if (!sceneFounded)
        {
            GameEconomy.curentLevel = levelConfigurations[levelConfigurations.Count - 1];
            SceneManager.LoadScene(levelConfigurations[levelConfigurations.Count - 1].LoadingScene);
            
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
