using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulTestUI : MonoBehaviour
{
    [SerializeField] private Transform _playerRadarRingUI;
    [SerializeField] private Transform _soulRadarRingUI;

    private GameParemeters _gameParemeters;

    private void OnEnable()
    {
        AddListeners();
        ApplySettings(FindObjectOfType<GameSettings>().GetGameParemeters());
        TestUISettingsChanged();
    }

    private void OnDisable()
    {
        DeleteListeners();
    }

    private void AddListeners()
    {
        TestUI.TestUISettingsChanged += TestUISettingsChanged;
        GameSettings.GameSettingsLoaded += GameSettingsLoaded;
    }

    private void DeleteListeners()
    {
        TestUI.TestUISettingsChanged -= TestUISettingsChanged;
        GameSettings.GameSettingsLoaded -= GameSettingsLoaded;
    }

    private void GameSettingsLoaded(GameParemeters gameParemeters)
    {
        ApplySettings(gameParemeters);
    }

    private void ApplySettings(GameParemeters gameParemeters)
    {
        _playerRadarRingUI.localScale = new Vector3(_playerRadarRingUI.localScale.x * 2, 1f, gameParemeters.playerDetectionDistance * 2);
        _soulRadarRingUI.localScale = new Vector3(gameParemeters.soulDetectionDistance * 2, 1f, gameParemeters.soulDetectionDistance * 2);
    }

    private void TestUISettingsChanged()
    {
        _playerRadarRingUI.gameObject.SetActive(TestUI.testModeEnabled & TestUI.drawPlayerRadar);
        _soulRadarRingUI.gameObject.SetActive(TestUI.testModeEnabled & TestUI.drawSoulRadar);
    } 
}
