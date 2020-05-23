using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class GameInterfaceUI : MonoBehaviour
{
    public GameObject timerUI;
    public GameObject soulsUI;

    public GameEndUI gameEndUI;
    public Text receivedEnergy;
    public Image itemImage;
    public Text timer;
    public AudioSource audioSource;
    public AudioClip buttonClip;

    public void OnEnable()
    {
        GamePlay.GameTimerChanged += GameTimerChanged;
        GamePlay.GameEnded += GameEnded;
        GamePlay.ReceivedEnergyChanged += ReceivedEnergyChanged;

        if (GameEconomy.curentItem)
        {
            itemImage.sprite = GameEconomy.curentItem.itemImage;
            if (GameEconomy.curentItem.GetParameterByEnum(GameParameter.PlayerLifesCount) != null)
            {
                Player.PlayerDamaged += PlayerDamaged;
            } 
        }

        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.AddListener(() => ButtonPressed(button));
        }
    }

    private void OnDisable()
    {
        GamePlay.GameTimerChanged -= GameTimerChanged;
        GamePlay.GameEnded -= GameEnded;
        Player.PlayerDamaged -= PlayerDamaged;
        GamePlay.ReceivedEnergyChanged -= ReceivedEnergyChanged;
    }

    private void GameEnded(GameEndData gameEndData)
    {
        gameEndUI.SetConfiguration(gameEndData);
    }

    private void GameTimerChanged(int curentGameTime)
    {
        if (curentGameTime < 0) timerUI.SetActive(false);
        timer.text = curentGameTime.ToString();
    }

    private void PlayerDamaged(int curentLifes)
    {
        itemImage.gameObject.SetActive(false);
    }

    private void ReceivedEnergyChanged(int _receivedEnergy, int _needEnergy)
    {
        if (_needEnergy < 0) soulsUI.SetActive(false);
            receivedEnergy.text = _receivedEnergy.ToString() + "/" + _needEnergy;
        if (_receivedEnergy >= _needEnergy)
            receivedEnergy.color = new Color(0f, 1f, 0f, 1f);
    }

    public void ButtonPressed(Button button)
    {
        button.transform.DOPunchScale(button.transform.localScale * 0.1f, 0.1f, 0, 0);
        audioSource.PlayOneShot(buttonClip);
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

    public void StartNextLevel()
    {
        List<LevelConfiguration> levelConfigurations = Resources.LoadAll("Levels", typeof(LevelConfiguration)).Cast<LevelConfiguration>().ToList().OrderBy(u => u.levelNumber).ToList();

        for (int i = 0; i < levelConfigurations.Count; i++)
        {
            Debug.Log(levelConfigurations[i] + " " + GameEconomy.curentLevel);
            if (i != levelConfigurations.Count && levelConfigurations[i].LoadingScene == SceneManager.GetActiveScene().name)
            {
                GameEconomy.curentLevel = levelConfigurations[i + 1];
                SceneManager.LoadScene(levelConfigurations[i + 1].LoadingScene);
            }
        }
    }

    public void SetPauseEnabled()
    {
        FindObjectOfType<GamePlay>().SetPauseEnabled();
    }

    public void SetPauseDisabled()
    {
        FindObjectOfType<GamePlay>().SetPauseDisabled();
    }

    public void PushButtonScale(Transform button)
    {
        button.DOPunchScale(button.localScale * 0.1f, 0.1f, 0, 0);
    }
}
