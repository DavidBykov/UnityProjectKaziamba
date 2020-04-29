using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneUI : MonoBehaviour
{
    public GameObject playButton;
    public Text loadingText;
    public Text loadingHint;
    public Text levelNameText;

    private AsyncOperation asyncLoad;

    private LevelConfiguration _loadingLevelConfiguration;

    public void SetLoadingConfiguration(LevelConfiguration levelConfiguration)
    {
        _loadingLevelConfiguration = levelConfiguration;
    }

    private void OnEnable()
    {
        StartCoroutine("LoadAsyncScene");
    }

    private IEnumerator LoadAsyncScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(_loadingLevelConfiguration.LoadingScene);
        asyncLoad.allowSceneActivation = false;

        //levelNameText.text _loadingLevelConfiguration.LevelName;
        levelNameText.text = _loadingLevelConfiguration.LevelName;
        loadingHint.text = _loadingLevelConfiguration.LevelLoadingHint;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                playButton.SetActive(true);
            }

            loadingText.text = "ЗАГРУЗКА " + (Mathf.Round(asyncLoad.progress) * 100) + "%";
            yield return null;
        }
    }

    public void StartNewScene()
    {
        asyncLoad.allowSceneActivation = true;
    }
}
