using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
