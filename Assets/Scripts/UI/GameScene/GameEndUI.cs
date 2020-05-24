using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct GameEndData
{
    public bool playerDie;
    public bool win;
    public float finalScore;
    public float needScore;
}

public class GameEndUI : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip winSound;
    public AudioClip loseSound;

    public Text status;
    public Text finalScore;
    public Text needScore;
    public GameObject nextLevelButton;

    public void SetConfiguration(GameEndData gameEndData)
    {
        gameObject.SetActive(true);
        finalScore.text = gameEndData.finalScore.ToString();
        needScore.text = gameEndData.needScore.ToString(); if (gameEndData.needScore < 0) needScore.text = "НЕ ВАЖНО";

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().enabled = false;

        if (gameEndData.win)
        {
            audioSource.PlayOneShot(winSound);
            status.text = "ВЫ ВЫИГРАЛИ!";
            nextLevelButton.SetActive(true);
        } else
        {
            audioSource.PlayOneShot(loseSound);
            status.text = "ВЫ ПРОИГРАЛИ!";
        }
    }
}
