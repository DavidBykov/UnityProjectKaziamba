using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDescriptionUI : MonoBehaviour
{
    public Text levelNameText;
    public Text levelDescriptionText;
    public GameObject hint;

    private string sceneName;
    private LevelConfiguration _levelConfiguration;
    public Image artefactImage;
    public Sprite WholeSprite;

    private void OnEnable()
    {
        levelNameText.text = _levelConfiguration.LevelName;
        levelDescriptionText.text = _levelConfiguration.LevelDescription;

        if (GameEconomy.curentItem)
        {
            artefactImage.sprite = GameEconomy.curentItem.itemImage;
        } else
        {
            artefactImage.sprite = WholeSprite;
        }

        if (_levelConfiguration.showHint && !_levelConfiguration.completed) hint.SetActive(true); else hint.SetActive(false);
    }

    public void SetLevelConfiguration(LevelConfiguration levelConfiguration)
    {
        _levelConfiguration = levelConfiguration;
    }

    public void SetLoadLevel(string scene)
    {
        sceneName = scene;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(_levelConfiguration.LoadingScene);
    }
}
