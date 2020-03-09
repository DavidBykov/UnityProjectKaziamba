using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TestUI : MonoBehaviour
{
    public delegate void OnTestUISettingsChanged();
    public static event OnTestUISettingsChanged TestUISettingsChanged;

    public Text testModeEnabledText;
    public Text isometryText;
    public Text playerRadarText;
    public Text soulRadarText;

    public bool testModeEnabledInspector;
    public bool drawPlayerRadarInspector;
    public bool drawSoulInspector;
    public bool isometryEnabledInspector;

    public static bool testModeEnabled;
    public static bool drawPlayerRadar;
    public static bool drawSoulRadar;
    public static bool isometryEnabled;

    //private void Start()
    //{
    //    OnValidate();
    //}

    private void OnValidate()
    {
        testModeEnabled = testModeEnabledInspector;
        drawPlayerRadar = drawPlayerRadarInspector;
        drawSoulRadar = drawSoulInspector;
        isometryEnabled = isometryEnabledInspector;
        UpdateUI();
    }

    public void ChangeTestModeStatus()
    {
        testModeEnabled = !testModeEnabled;
        UpdateUI();
    }

    public void ChangeIsometry()
    {
        isometryEnabled = !isometryEnabled;
        UpdateUI();
    }

    public void ChangeDrawPlayerRadar()
    {
        drawPlayerRadar = !drawPlayerRadar;
        UpdateUI();
    }

    public void ChangeDrawSoulRadar()
    {
        drawSoulRadar = !drawSoulRadar;
        UpdateUI();
    }

    private void UpdateUI()
    {
        testModeEnabledText.text = testModeEnabled.ToString();
        isometryText.text = isometryEnabled.ToString();
        playerRadarText.text = drawPlayerRadar.ToString();
        soulRadarText.text = drawSoulRadar.ToString();

        TestUISettingsChanged?.Invoke();
    }
}
