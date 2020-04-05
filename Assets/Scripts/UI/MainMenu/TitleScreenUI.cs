using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenUI : MonoBehaviour
{
    public string menuSceneName;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
