using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDescriptionUI : MonoBehaviour
{
    private string sceneName;
    public void SetLoadLevel(string scene)
    {
        sceneName = scene;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(sceneName);
    }
}
