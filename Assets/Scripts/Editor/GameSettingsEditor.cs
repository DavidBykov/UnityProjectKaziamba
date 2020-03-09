using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameSettings))]
public class GameSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameSettings myTarget = (GameSettings)target;

        if (GUILayout.Button("Apply settings")) {
            myTarget.ApplySettings();
        }
    }
}
