using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneUI : MonoBehaviour
{
    public GameObject playButton;
    public GameObject readButton;
    public Text loadingText;
    public Text loadingHint;
    public Text levelNameText;
    public Image comics;

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
        if(_loadingLevelConfiguration.LevelLoadingHint.Length > 0) loadingHint.text = _loadingLevelConfiguration.LevelLoadingHint[Random.Range(0, _loadingLevelConfiguration.LevelLoadingHint.Length)];

        if (_loadingLevelConfiguration.comics != null)
        {
            comics.sprite = _loadingLevelConfiguration.comics;
            comics.color = new Color(255,255,255,1);
        } else
        {
            comics.color = new Color(0, 0, 0, 0);
        }

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                if (_loadingLevelConfiguration.comics != null)
                {
                    readButton.SetActive(true);
                }
                else
                {
                    playButton.SetActive(true);
                }
                yield break;
            }

            if (_loadingLevelConfiguration.comics == null)
                loadingText.text = "ЗАГРУЗКА " + (Mathf.Round(asyncLoad.progress) * 100) + "%";
            else
                loadingText.text = "";

            yield return null;
        }
    }

    public void StartNewScene()
    {
        asyncLoad.allowSceneActivation = true;
    }
}
