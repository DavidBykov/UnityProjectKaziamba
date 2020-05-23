using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePhrases : MonoBehaviour
{
    public string[] phrases;
    public Text phraseText;

    private void OnEnable()
    {
        phraseText.text = phrases[Random.Range(0, phrases.Length)];
    }
}
