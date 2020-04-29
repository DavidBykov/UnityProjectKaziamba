using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameInterfaceUI : MonoBehaviour
{
    public Image itemImage;
    public void OnEnable()
    {
        if (GameEconomy.curentItem)
        {
            itemImage.sprite = GameEconomy.curentItem.itemImage;
        }    
    }

    public void LoadGameScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartNextLevel()
    {
        List<LevelConfiguration> levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList();

        for(int i = 0; i < levelConfigurations.Count; i++)
        {
            Debug.Log(levelConfigurations[i] + " " + GameEconomy.curentLevel);
            if (i != levelConfigurations.Count && levelConfigurations[i].LoadingScene == SceneManager.GetActiveScene().name)
            {
                GameEconomy.curentLevel = levelConfigurations[i + 1];
                SceneManager.LoadScene(levelConfigurations[i + 1].LoadingScene);
            }
        }
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void SetPauseEnabled()
    {
        FindObjectOfType<GamePlay>().SetPauseEnabled();
    }

    public void SetPauseDisabled()
    {
        FindObjectOfType<GamePlay>().SetPauseDisabled();
    }
}
