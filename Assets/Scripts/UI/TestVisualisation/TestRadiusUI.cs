using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRadiusUI : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private GameParemeters _gameParemeters;

    private void OnEnable()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        AddListeners();
        TestUISettingsChanged();
    }

    private void OnDisable()
    {
        DeleteListeners();
    }

    private void AddListeners()
    {
        TestUI.TestUISettingsChanged += TestUISettingsChanged;
    }

    private void DeleteListeners()
    {
        TestUI.TestUISettingsChanged -= TestUISettingsChanged;
    }

    private void TestUISettingsChanged()
    {
        _meshRenderer.enabled = TestUI.testModeEnabled & TestUI.drawPlayerRadar;
    }
}
