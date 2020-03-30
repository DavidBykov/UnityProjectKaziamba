using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        _gameTime = (int)FindObjectOfType<GameSettings>().GetGameParemeters().gameTime;
        _soulsMaxCount = (int)FindObjectOfType<GameSettings>().GetGameParemeters().startSoulsCount;

        soulsText.text = souls.ToString() + "/" + _soulsMaxCount;
        StartCoroutine(Timer());   
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
                if (i == 0) if (souls >= _soulsMaxCount)
                {
                    winPanel.SetActive(true);
                    PlayDefeatSound();
                }
                else
                {
                    losePanel.SetActive(true);
                    PlayDefeatSound();
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

    [ContextMenu("PlayDefeatSound")]
    public void PlayDefeatSound()
    {
        gameMusic.PlayOneShot(defeatSound);
    }
}
