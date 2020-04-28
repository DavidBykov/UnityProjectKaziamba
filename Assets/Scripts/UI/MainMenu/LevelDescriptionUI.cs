using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class LevelDescriptionUI : MonoBehaviour
{
    public Text levelNameText;
    public Text levelTimeText;
    public Text levelSoulsText;
    public Text levelEnergyText;
    public Text levelDescriptionText;
    public GameObject hint;

    private string sceneName;
    private LevelConfiguration _levelConfiguration;
    public Image artefactImage;
    public Sprite WholeSprite;
    public GameObject loadingScreen;
    public ItemDescriptionUI itemDescriptionUI;

    public LoadingSceneUI loadingSceneUI;
    public Image itemImage;
    public GameObject hideItemImage;


    public List<ItemConfiguration> itemConfigurations = new List<ItemConfiguration>();
    private ItemConfiguration curentItem;


    private void OnEnable()
    {
        levelNameText.text = _levelConfiguration.LevelName;

        if (_levelConfiguration.LevelGameSettings.gameTime == -1) levelTimeText.text = "ПОКА НЕ УСТАНЕШЬ"; else levelTimeText.text = _levelConfiguration.LevelGameSettings.gameTime.ToString();

        levelSoulsText.text = _levelConfiguration.LevelGameSettings.soulsOnLevel.ToString();
        if (_levelConfiguration.LevelGameSettings.neededEnergy == -1) levelEnergyText.text = "НЕ ВАЖНО"; else levelEnergyText.text = _levelConfiguration.LevelGameSettings.neededEnergy.ToString();

        levelDescriptionText.text = _levelConfiguration.LevelDescription;

        itemConfigurations = Resources.LoadAll("Items", typeof(ItemConfiguration)).Cast<ItemConfiguration>().ToList();
        if (GameEconomy.curentItem)
        {
            curentItem = GameEconomy.curentItem;
        } else
        {
            if (itemConfigurations.Count > 0) curentItem = itemConfigurations.First();
        }
        
        SetItem(curentItem);

        if (_levelConfiguration.showHint && !SaveSystem.LoadLevelStatucByID(_levelConfiguration.levelSaveLoadID)) hint.SetActive(true); else hint.SetActive(false);
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
        loadingSceneUI.SetLoadingConfiguration(_levelConfiguration);
        loadingSceneUI.gameObject.SetActive(true);
    }

    private void SetItem(ItemConfiguration itemConfiguration)
    {
        curentItem = itemConfiguration;
        itemImage.sprite = curentItem.itemImage;
        if (curentItem.bought)
        {
            hideItemImage.SetActive(false);
            GameEconomy.curentItem = curentItem;
        }
        else
        {
            hideItemImage.SetActive(true);
            GameEconomy.curentItem = null;
        }
    }

    public void NextItem()
    {
        if (itemConfigurations.Count == 0) return;

        int index = itemConfigurations.IndexOf(curentItem);
        if (index + 1 > itemConfigurations.Count - 1) {
            SetItem(itemConfigurations.First());
        } else
        {
            SetItem(itemConfigurations[index + 1]);
        }
    }

    public void PrevItem()
    {
        int index = itemConfigurations.IndexOf(curentItem);
        if (index - 1 < 0)
        {
            SetItem(itemConfigurations.Last());
        }
        else
        {
            SetItem(itemConfigurations[index - 1]);
        }
    }

    public void OpenItemPanel()
    {
        itemDescriptionUI.SetItemConfiguration(curentItem);
        itemDescriptionUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
