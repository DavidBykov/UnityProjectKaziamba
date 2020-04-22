using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [HideInInspector] public float _smooth;
    [SerializeField] private Camera _camera;
    private Transform _player;

    private Joystick _joystick;
    private GameParemeters _gameSettings;

    void OnEnable()
    {
        AddListeners();
       
        _joystick = FindObjectOfType<Joystick>();
        
        _player = FindObjectOfType<Player>().transform;
        //TestUI.TestUISettingsChanged += TestUISettingsChanged;
        
    }

    private void AddListeners()
    {
        GameSettings.GameSettingsLoaded += GameSettingsLoaded;
    }

    private void GameSettingsLoaded(GameParemeters gameParemeters)
    {
        _smooth = gameParemeters.cameraSmoothPosition;
        _gameSettings = gameParemeters;
        SetHeight();
    }

    private void OnDisable()
    {
        TestUI.TestUISettingsChanged -= TestUISettingsChanged;
    }

    private void TestUISettingsChanged()
    {
        _camera.orthographic = TestUI.isometryEnabled;
        SetHeight();
    }

    private void SetHeight()
    {
        if (!_camera) return;

        if (_camera.orthographic)
            _camera.orthographicSize = _gameSettings.cameraHeightDistance / 2;
        else
            _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y, -_gameSettings.cameraHeightDistance);
    }

    void FixedUpdate()
    {
        if (_gameSettings)
        {
            Vector3 cameraPosition = new Vector3(_gameSettings.cameraForwardHorizontalDistance * _joystick.Horizontal, transform.localPosition.y, _gameSettings.cameraForwardVerticalDistance * _joystick.Vertical);

            transform.localPosition = Vector3.Lerp(transform.localPosition, cameraPosition, Time.timeScale * _gameSettings.cameraSmoothPosition);
        }
    }
}
