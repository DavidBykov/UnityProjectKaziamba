using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct GameEndData
{
    public bool win;
    public string finalScore;
    public string needScore;
}

public class GameEndUI : MonoBehaviour
{
    public Text status;
    public Text finalScore;
    public Text needScore;
    public GameObject nextLevelButton;

    public void SetConfiguration(GameEndData gameEndData)
    {
        gameObject.SetActive(true);
        finalScore.text = gameEndData.finalScore.ToString();
        needScore.text = gameEndData.needScore.ToString();
        if (gameEndData.win)
        {
            status.text = "ВЫ ВЫЙГРАЛИ!";
            nextLevelButton.SetActive(true);
        } else
        {
            status.text = "ВЫ ПРОИГРАЛИ!";
        }
    }
}
