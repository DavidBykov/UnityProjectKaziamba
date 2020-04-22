using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public delegate void OnGameSettingsLoaded(GameParemeters gameParemeters);
    public static event OnGameSettingsLoaded GameSettingsLoaded;

    [SerializeField] private GameParemeters _gameParemeters;

    private void Awake()
    {
        if (GameEconomy.curentLevel) _gameParemeters = GameEconomy.curentLevel.LevelGameSettings;
        GameSettingsLoaded?.Invoke(_gameParemeters);
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        GameSettingsLoaded?.Invoke(_gameParemeters);
    }

    public GameParemeters GetGameParemeters()
    {
        return _gameParemeters;
    }

    public void ApplySettings()
    {
        GameSettingsLoaded?.Invoke(_gameParemeters);
    }
}
