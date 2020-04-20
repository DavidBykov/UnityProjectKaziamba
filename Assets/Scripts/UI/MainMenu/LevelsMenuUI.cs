using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelsMenuUI : MonoBehaviour
{
    public string scrollButtonPrefix;
    public GameObject buttonsPanelPrefab;
    public GameObject buttonPrefab;
    public LevelDescriptionUI levelDescriptionUI;

    public Transform contentTransform;
    public List<LevelConfiguration> levelConfigurations = new List<LevelConfiguration>();

    private Dictionary<Button, LevelConfiguration> buttonLevelPairs = new Dictionary<Button, LevelConfiguration>();

    public LoadingSceneUI loadingSceneUI;

    public int needHoles;

    public void OnEnable()
    {
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            Destroy(contentTransform.GetChild(i).gameObject);
        }

        levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList();
        buttonLevelPairs.Clear();

        //GameObject newPanel = Instantiate(buttonsPanelPrefab, contentTransform);
        //for (int i = 0; i < levelConfigurations.Count; i++)
        //{
        //    if (i % 3 == 0 && i != 0)
        //    {
        //        newPanel = Instantiate(buttonsPanelPrefab, contentTransform);
        //    }


        //    GameObject newButton = Instantiate(buttonPrefab, newPanel.transform);
        //    Button newUIButton = newButton.GetComponent<Button>();

        //    if ((i > 0 && levelConfigurations[i - 1].completed) || i == 0)
        //    {
        //        newUIButton.onClick.AddListener(() => ButtonClicked(newUIButton));
        //    }
        //    else if (i != 0)
        //    {
        //        newUIButton.transform.Find("Image").gameObject.SetActive(false);
        //    }

        //    buttonLevelPairs.Add(newUIButton, levelConfigurations[i]);
        //}

        for (int i = 0; i < levelConfigurations.Count; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, contentTransform);

            Button newUIButton = newButton.GetComponent<Button>();

            if ((i > 0 && levelConfigurations[i - 1].completed) || i == 0)
            {
                newUIButton.onClick.AddListener(() => ButtonClicked(newUIButton));
            }
            else if (i != 0)
            {
                newUIButton.transform.Find("Image").gameObject.SetActive(false);
            }

            buttonLevelPairs.Add(newUIButton, levelConfigurations[i]);
        }

        for (int i = 0; i < needHoles - (levelConfigurations.Count % 3 - 1); i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, contentTransform);
            newButton.transform.Find("Image").gameObject.SetActive(false);
        }
    }

    private void ButtonClicked(Button clickedButton)
    {
        LevelConfiguration level;
        buttonLevelPairs.TryGetValue(clickedButton, out level);
        if (level)
        {
            GameEconomy.curentLevel = level;
            Debug.Log(GameEconomy.curentLevel);
            if (level.startWithoutDescription)
            {
                loadingSceneUI.SetLoadingConfiguration(level);
                loadingSceneUI.gameObject.SetActive(true);
            }
            else
            {
                levelDescriptionUI.SetLevelConfiguration(level);
                levelDescriptionUI.gameObject.SetActive(true);
            }
        }
    }

    public void OpenAllLevels()
    {
        levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList();
        foreach(LevelConfiguration levelConfiguration in levelConfigurations)
        {
            levelConfiguration.completed = true;
        }

        OnEnable();
    }


    public void CloseAllLevels()
    {
        levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList();
        foreach (LevelConfiguration levelConfiguration in levelConfigurations)
        {
            levelConfiguration.completed = false;
        }

        OnEnable();
    }
}
