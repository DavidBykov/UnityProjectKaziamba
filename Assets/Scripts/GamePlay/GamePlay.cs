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
    [HideInInspector] public int _soulsMaxCount;

    public GameObject winPanel;
    public GameObject losePanel;

    private int souls = 0;

    private bool gamePaused;

    public bool startFromPause;

    private void Awake()
    {
        instance = this;
    }

    private List<Soul> allSoulsOnGameField;

    void Start()
    {
        Application.targetFrameRate = 60;
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

        _soulsMaxCount = (int)FindObjectOfType<GameSettings>().GetGameParemeters().startSoulsCount;

        soulsText.text = souls.ToString() + "/" + _soulsMaxCount;
        if(_gameTime != -1) StartCoroutine(Timer());
        if (_soulsMaxCount == -1) _soulsMaxCount = FindObjectsOfType<Soul>().Length;
        if (startFromPause) SetPauseEnabled();

        allSoulsOnGameField = FindObjectsOfType<Soul>().ToList();
        foreach(Soul soul in allSoulsOnGameField)
        {
            soul.SoulDeath += SoulDeath;
        }
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
                    if (souls >= _soulsMaxCount)
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
                    GameEconomy.AddPlayerMoney(souls);
                    if(GameEconomy.curentItem) GameEconomy.curentItem.bought = false;
                    GameEconomy.curentItem = null;
                }
                i--;    
            }
            yield return new WaitForSecondsRealtime(1f);
        }
        
    }

    public void AddSouls()
    {
        souls++;
        soulsText.text = souls.ToString() + "/" + _soulsMaxCount;
        soulsText.rectTransform.DOPunchScale(gameTimeText.rectTransform.localScale / 8, 0.1f, 0, 1);

        if(souls >= _soulsMaxCount)
        {
            winPanel.SetActive(true);
            GameEconomy.curentLevel.completed = true;
        }

        if(allSoulsOnGameField.Count <= 0 && souls < _soulsMaxCount)
        {
            losePanel.SetActive(true);
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
        gameMusic.Pause();
        gamePaused = true;
    }

    public void SetPauseDisabled()
    {
        Time.timeScale = 1f;
        gameMusic.UnPause();
        gamePaused = false;
    }

    [ContextMenu("PlayDefeatSound")]
    public void PlayDefeatSound()
    {
        if(defeatSound) gameMusic.PlayOneShot(defeatSound);
    }
}
