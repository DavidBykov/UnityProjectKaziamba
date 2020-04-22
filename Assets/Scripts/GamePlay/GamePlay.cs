using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Linq;

public class GamePlay : MonoBehaviour
{
    public static GamePlay instance;

    public AudioSource gameMusic;
    public AudioClip defeatSound;
    
    public Text gameTimeText;
    public Text soulsText;

    private int _gameTime;
    [HideInInspector] public int _needEnergyToCompleteLevel;
    public int _cathedSouls = 0;

    public GameObject winPanel;
    public GameObject losePanel;

    private int receivedEnergy = 0;

    private bool gamePaused;

    public bool startFromPause;
    private int initialSoulsOnField;

    private void Awake()
    {
        instance = this;
    }

    private List<Soul> allSoulsOnGameField;
    private GameParemeters _gameParemeters;

    private int curentSameTimeSoulsCount = 0;
    private bool modifierInWork;

    public bool playerKilled;

    void Start()
    {
        Application.targetFrameRate = 60;
        _gameParemeters = FindObjectOfType<GameSettings>().GetGameParemeters();
        _gameTime = (int)FindObjectOfType<GameSettings>().GetGameParemeters().gameTime;

        if (GameEconomy.curentItem)
        {
            foreach (ChangingParameter changingParameter in GameEconomy.curentItem.changingParameters)
            {
                if (changingParameter.gameParameter == GameParameter.GameTime)
                {
                    if (changingParameter.useAsPercent)
                        _gameTime *= (int)changingParameter.value;
                    else
                        _gameTime += (int)changingParameter.value;
                }
            }
        }

        _needEnergyToCompleteLevel = (int)FindObjectOfType<GameSettings>().GetGameParemeters().neededEnergy;

        soulsText.text = receivedEnergy.ToString() + "/" + _needEnergyToCompleteLevel;
        if(_gameTime != -1) StartCoroutine(Timer());
        if (_needEnergyToCompleteLevel == -1) _needEnergyToCompleteLevel = FindObjectsOfType<Soul>().Length;
        if (startFromPause) SetPauseEnabled();

        allSoulsOnGameField = FindObjectsOfType<Soul>().ToList();
        initialSoulsOnField = allSoulsOnGameField.Count();

        foreach (Soul soul in allSoulsOnGameField)
        {
            soul.SoulDeath += SoulDeath;
        }

        Debug.Log(GameEconomy.curentLevel.LoadingScene);
    }

    private void SoulDeath(Soul soul)
    {
        allSoulsOnGameField.Remove(soul);
    }

    private IEnumerator Timer()
    {
        int i = _gameTime;

        while (i >= 0)
        {
            if (!gamePaused)
            {
                
                gameTimeText.text = i.ToString();
                gameTimeText.rectTransform.DOPunchScale(gameTimeText.rectTransform.localScale / 8, 0.1f, 0, 1);
                if (i == 0)
                {
                    if (receivedEnergy >= _needEnergyToCompleteLevel)
                    {
                        winPanel.SetActive(true);
                        PlayDefeatSound();
                        GameEconomy.curentLevel.completed = true;
                    }
                    else
                    {
                        losePanel.SetActive(true);
                        PlayDefeatSound();
                    }
                    GameEconomy.AddPlayerMoney(receivedEnergy);
                    if(GameEconomy.curentItem) GameEconomy.curentItem.bought = false;
                    GameEconomy.curentItem = null;
                }
                i--;    
            }
            yield return new WaitForSecondsRealtime(1f);
        }
        
    }

    public void TryAddEnergy(int energyCount)
    {
        _cathedSouls++;
        if (modifierInWork) curentSameTimeSoulsCount++;
        if (!modifierInWork) StartCoroutine("WaitingToAnotherDeathSoul");
    }

    private IEnumerator WaitingToAnotherDeathSoul()
    {
        curentSameTimeSoulsCount = 1;
        modifierInWork = true;
        yield return new WaitForSeconds(_gameParemeters.timeBetwenSoulsCatch);

        AddEnergyByModifier();

        modifierInWork = false;
    }

    private void AddEnergyByModifier()
    {
        int addFinalEnergy = 0;

        switch (curentSameTimeSoulsCount)
        {
            case 1: addFinalEnergy = 1; break;
            case 2: addFinalEnergy = 3; break;
            case 3: addFinalEnergy = 5; break;
            case 4: addFinalEnergy = 7; break;
            case 5: addFinalEnergy = 10; break;
        }

        AddEnergy(addFinalEnergy);
        curentSameTimeSoulsCount = 0;
    }


    public void AddEnergy(int energyCount)
    {
        Debug.Log("Вызвано добавление энергии");

        receivedEnergy += energyCount;
        soulsText.text = receivedEnergy.ToString() + "/" + _needEnergyToCompleteLevel;
        soulsText.rectTransform.DOPunchScale(gameTimeText.rectTransform.localScale / 8, 0.1f, 0, 1);

        CheckGameEndedCondition();
    }

    public void CheckGameEndedCondition()
    {
        if ((!_gameParemeters.useCatchedSoulsAsCompleteLevelCondition && receivedEnergy >= _needEnergyToCompleteLevel) || (_gameParemeters.useCatchedSoulsAsCompleteLevelCondition && _cathedSouls >= initialSoulsOnField))
        {
            winPanel.SetActive(true);
            if (GameEconomy.curentItem) GameEconomy.curentItem.bought = false;
            GameEconomy.curentItem = null;
            GameEconomy.curentLevel.completed = true;
        }

        if (allSoulsOnGameField.Count <= 0 && receivedEnergy < _needEnergyToCompleteLevel)
        {
            losePanel.SetActive(true);
        }

        if (playerKilled)
        {
            losePanel.SetActive(true);
            StopCoroutine("Timer");
            //SetPauseEnabled();
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadGameScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
            Time.timeScale = 0f;
            gameMusic.Pause();
            gamePaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            gameMusic.UnPause();
            gamePaused = false;
        }
    }

    public void SetPauseEnabled()
    {
        Time.timeScale = 0f;
        if (gameMusic) gameMusic.Pause();
        gamePaused = true;
    }

    public void SetPauseDisabled()
    {
        Time.timeScale = 1f;
        if(gameMusic) gameMusic.UnPause();
        gamePaused = false;
    }

    [ContextMenu("PlayDefeatSound")]
    public void PlayDefeatSound()
    {
        if(defeatSound) gameMusic.PlayOneShot(defeatSound);
    }
}
