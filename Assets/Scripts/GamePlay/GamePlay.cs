using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Linq;

public class GamePlay : MonoBehaviour
{
    public delegate void OnGameTimerChanged(int curentGameTime);
    public static event OnGameTimerChanged GameTimerChanged;

    public delegate void OnGameEnded(GameEndData gameEndData);
    public static event OnGameEnded GameEnded;

    public delegate void OnGameChangePause(bool paused);
    public static event OnGameChangePause GameChangePause;

    public delegate void OnReceivedEnergyChanged(int receivedEnergy, int needEnergy);
    public static event OnReceivedEnergyChanged ReceivedEnergyChanged;

    // Полученные настройки игры
    private int _gameTime;
    private int _needEnergyToCompleteLevel;
    private int _cathedSouls;
    private int _receivedEnergy = 0;
    private bool _startFromPause;

    [SerializeField] private int _initialSoulsOnField;
    [SerializeField] private int _curentSameTimeSoulsCount;
    private bool _modifierInWork;

    private bool _gamePaused;
    private bool _playerKilled;
    private bool _gameEnded;

    private List<Soul> _allSoulsOnGameField;
    private GameParemeters _gameParemeters;
    private int soulsCount;

    private void OnEnable()
    {
        Player.PlayerDie += PlayerDie;
        GameSettings.GameSettingsLoaded += GameSettingsLoaded;
    }

    private void OnDisable()
    {
        Player.PlayerDie -= PlayerDie;
        GameSettings.GameSettingsLoaded -= GameSettingsLoaded;
    }

    private void PlayerDie()
    {
        _playerKilled = true;
        CheckGameEndedCondition(_gameParemeters.gameEndCondition);
    }

    private void FindAndStartListeningSouls()
    {
        _allSoulsOnGameField = FindObjectsOfType<Soul>().ToList();
        _initialSoulsOnField = _allSoulsOnGameField.Count();

        foreach (Soul soul in _allSoulsOnGameField)
        {
            soul.SoulDeath += SoulDeath;
        }
    }

    private void GameSettingsLoaded(GameParemeters gameParemeters)
    {
        Debug.Log("Настройки игры загружены!");

        _gameParemeters = gameParemeters;
        _gameTime = (int)gameParemeters.gameTime;
        _needEnergyToCompleteLevel = (int)gameParemeters.neededEnergy;

        if (GameEconomy.curentItem)
        {
            _gameTime = (int)GameEconomy.curentItem.TryModifyParameter(GameParameter.GameTime, _gameTime);
        }

        Application.targetFrameRate = 60;

        if (_startFromPause) SetPauseEnabled(); else SetPauseDisabled();
        FindAndStartListeningSouls();

        StopCoroutine("Timer");

        if (_gameTime != -1)
        {
            StartCoroutine("Timer");
        }
        else
        {
            AddEnergy(0);
            GameTimerChanged?.Invoke(-1);
        }
    }

    private void SoulDeath(Soul soul, bool addEnergy)
    {
        if (addEnergy) TryAddEnergy(_gameParemeters.energyForCatchSoul);
        _allSoulsOnGameField.Remove(soul);
        CheckGameEndedCondition(_gameParemeters.gameEndCondition);
    }

    private IEnumerator Timer()
    {
        int i = (int)_gameTime;

        AddEnergy(0);

        while (i >= 0)
        {
            if (!_gamePaused)
            {
                GameTimerChanged?.Invoke(i);
                i--;
            }
            yield return new WaitForSecondsRealtime(1f);
        }
        _gameEnded = true;

        CheckGameEndedCondition(_gameParemeters.gameEndCondition);
        yield return null;
    }

    public void TryAddEnergy(int energyCount)
    {
        _cathedSouls++;
        if (_modifierInWork) _curentSameTimeSoulsCount++;
        if (!_modifierInWork) StartCoroutine("WaitingToAnotherDeathSoul");
    }

    private IEnumerator WaitingToAnotherDeathSoul()
    {
        _curentSameTimeSoulsCount = 1;
        _modifierInWork = true;
        yield return new WaitForSeconds(_gameParemeters.timeBetwenSoulsCatch);

        AddEnergyByModifier();

        _modifierInWork = false;
    }

    private void AddEnergyByModifier()
    {
        int addFinalEnergy = 0;

        switch (_curentSameTimeSoulsCount)
        {
            case 1: addFinalEnergy = 1; break;
            case 2: addFinalEnergy = 3; break;
            case 3: addFinalEnergy = 5; break;
            case 4: addFinalEnergy = 7; break;
            case 5: addFinalEnergy = 10; break;
        }
        if (_curentSameTimeSoulsCount > 5) addFinalEnergy = 10;

        AddEnergy(addFinalEnergy);
        _curentSameTimeSoulsCount = 0;
    }

    public void AddEnergy(int energyCount)
    {
        _receivedEnergy += energyCount;

        ReceivedEnergyChanged?.Invoke(_receivedEnergy, _needEnergyToCompleteLevel);
        CheckGameEndedCondition(_gameParemeters.gameEndCondition);
    }

    public void CheckGameEndedCondition(GameEndCondition gameEndCondition)
    {
        Debug.Log("Проверяю игру на условие выйгрыша");

        if (gameEndCondition == GameEndCondition.ReceivedRightEnergyAmount)
        {
            if(_allSoulsOnGameField.Count <= 0)
            {
                if (_receivedEnergy <_needEnergyToCompleteLevel)
                {
                    GameEnd(false);
                } else {
                    GameEnd(true);
                }
            }
        }

        if (gameEndCondition == GameEndCondition.CatchedAllSouls)
        {
            if(_cathedSouls == _initialSoulsOnField)
            {
                GameEnd(true);
                return;
            } else if((_cathedSouls < _initialSoulsOnField) && _gameEnded)
            {
                GameEnd(false);
                return;
            }
        }

        if(gameEndCondition == GameEndCondition.ReceivedRightEnergyAmount && _gameEnded)
        {
            if(_receivedEnergy >= _needEnergyToCompleteLevel)
            {
                GameEnd(true);
                return;
            }
            else 
            {
                GameEnd(false);
                return;
            }
        }

        if (_playerKilled)
        {
            GameEnd(false);
            return;
        }
    }

    private void GameEnd(bool win)
    {
        StopCoroutine("Timer");
        GameEndData gameEndData = new GameEndData();
        gameEndData.win = win;
        gameEndData.finalScore = _receivedEnergy;
        gameEndData.needScore = _needEnergyToCompleteLevel;
        gameEndData.playerDie = false;

        if (win)
        {
            SaveSystem.SaveLevelStatucByID(GameEconomy.curentLevel.levelSaveLoadID, true);
        }
        else
        {
            //SaveSystem.SaveLevelStatucByID(GameEconomy.curentLevel.levelSaveLoadID, false);
        }

        SetPauseEnabled();
        GameEnded(gameEndData);
    }
    
    public void RestartGame()
    {
        SetPauseDisabled();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadGameScene()
    {
        SetPauseDisabled();
        SceneManager.LoadScene("Game");
    }

    public void LoadMenuScene()
    {
        SetPauseDisabled();
        SceneManager.LoadScene("Menu");
    }

    public void SetPauseEnabled()
    {
        Time.timeScale = 0f;
        _gamePaused = true;
    }

    public void SetPauseDisabled()
    {
        Time.timeScale = 1f;
        _gamePaused = false;
    }
}
