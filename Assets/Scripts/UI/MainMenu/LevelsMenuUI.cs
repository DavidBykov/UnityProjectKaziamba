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

    public void OnEnable()
    {
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            Destroy(contentTransform.GetChild(i).gameObject);
        }

        levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList();
        buttonLevelPairs.Clear();

        GameObject newPanel = Instantiate(buttonsPanelPrefab, contentTransform);
        for(int i = 0; i < levelConfigurations.Count; i++)
        {
            if (i % 3 == 0 && i != 0)
            {
                newPanel = Instantiate(buttonsPanelPrefab, contentTransform);
            }

            GameObject newButton = Instantiate(buttonPrefab, newPanel.transform);
            Button newUIButton = newButton.GetComponent<Button>();
            newUIButton.onClick.AddListener(() => ButtonClicked(newUIButton));
            buttonLevelPairs.Add(newUIButton, levelConfigurations[i]);
        }   
    }

    private void ButtonClicked(Button clickedButton)
    {
        LevelConfiguration level;
        buttonLevelPairs.TryGetValue(clickedButton, out level);
        if (level)
        {
            if (level.startWithoutDescription)
            {
                SceneManager.LoadScene(level.LoadingScene);
            }
            else
            {
                levelDescriptionUI.SetLevelConfiguration(level);
                levelDescriptionUI.gameObject.SetActive(true);
            }
        }
    }

    public void LoadLevelByNumber(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
