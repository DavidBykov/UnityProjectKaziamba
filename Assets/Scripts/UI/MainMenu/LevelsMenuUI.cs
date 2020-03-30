using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelsMenuUI : MonoBehaviour
{
    public string scrollButtonPrefix;
    public Transform contentTransform;

    public void OnEnable()
    {
        int i = 1;
        foreach(Transform trans in contentTransform.GetComponentsInChildren<RectTransform>())
        {
            if (trans.name == scrollButtonPrefix)
            {
                trans.GetComponentInChildren<Text>().text = i.ToString();
                i++;
            }
        }       
    }

    public void LoadLevelByNumber(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
