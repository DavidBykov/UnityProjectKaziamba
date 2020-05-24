using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ComicsScrollsUI : MonoBehaviour
{
    public string scrollButtonPrefix;
    public GameObject buttonsPanelPrefab;
    public GameObject buttonPrefab;

    public Transform contentTransform;
    public List<LevelConfiguration> levelConfigurations = new List<LevelConfiguration>();

    private Dictionary<Button, Sprite> buttonLevelPairs = new Dictionary<Button, Sprite>();

    public ComicsMenuUI comicsMenuUI;

    public int needHoles;

    private List<Sprite> comics = new List<Sprite>();

    public void OnEnable()
    {
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            Destroy(contentTransform.GetChild(i).gameObject);
        }

        levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList().OrderBy(u => u.levelNumber).ToList();
        comics.Clear();

        foreach (LevelConfiguration levelConfiguration in levelConfigurations)
        {
            if(levelConfiguration.comics != null && SaveSystem.LoadLevelStatucByID(levelConfiguration.levelSaveLoadID))
            {
                comics.Add(levelConfiguration.comics);
            }
        }

        buttonLevelPairs.Clear();


        for (int i = 0; i < comics.Count; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, contentTransform);

            Button newUIButton = newButton.GetComponent<Button>();

            newUIButton.onClick.AddListener(() => ButtonClicked(newUIButton));
            newUIButton.transform.Find("LevelNumber").GetComponent<Text>().text = (i + 1).ToString();

            buttonLevelPairs.Add(newUIButton, comics[i]);
        }

        for (int i = 0; i < needHoles - (levelConfigurations.Count % 3 - 1); i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, contentTransform);
            newButton.transform.Find("Image").gameObject.SetActive(false);
        }
    }

    private void ButtonClicked(Button clickedButton)
    {
        Sprite spriteComic;
        buttonLevelPairs.TryGetValue(clickedButton, out spriteComic);
        if (spriteComic)
        {
            Debug.Log(GameEconomy.curentLevel);
            comicsMenuUI.SetComic(comics, spriteComic);
            comicsMenuUI.gameObject.SetActive(true);
        }
    }
}
