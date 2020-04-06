using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public delegate void OnGameSettingsLoaded(GameParemeters gameParemeters);
    public static event OnGameSettingsLoaded GameSettingsLoaded;

    [SerializeField] private GameParemeters _gameParemeters;

    private void Start()
    {
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
