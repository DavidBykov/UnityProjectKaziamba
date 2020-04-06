﻿using System.Collections;
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

    private void OnEnable()
    {
        levelNameText.text = _levelConfiguration.LevelName;
        levelDescriptionText.text = _levelConfiguration.LevelDescription;
        if (_levelConfiguration.showHint) hint.SetActive(true);
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
