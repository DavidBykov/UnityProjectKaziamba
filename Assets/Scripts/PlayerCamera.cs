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
        _smooth = FindObjectOfType<GameSettings>().GetGameParemeters().cameraSmoothPosition;
        _joystick = FindObjectOfType<Joystick>();
        _gameSettings = FindObjectOfType<GameSettings>().GetGameParemeters();
        _player = FindObjectOfType<Player>().transform;
        TestUI.TestUISettingsChanged += TestUISettingsChanged;
    }

    private void OnDisable()
    {
        TestUI.TestUISettingsChanged -= TestUISettingsChanged;
    }

    private void TestUISettingsChanged()
    {
        _camera.orthographic = TestUI.isometryEnabled;
    }

    void FixedUpdate()
    {
        Vector3 cameraPosition = new Vector3(_gameSettings.cameraForwardHorizontalDistance * _joystick.Horizontal, transform.localPosition.y, _gameSettings.cameraForwardVerticalDistance * _joystick.Vertical);

        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraPosition, Time.timeScale * _gameSettings.cameraSmoothPosition);
    }
}
