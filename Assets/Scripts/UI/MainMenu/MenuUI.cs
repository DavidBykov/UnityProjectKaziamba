using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonClip;

    private void OnEnable()
    {
        foreach(Button button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.AddListener(() => ButtonPressed(button));
        }
    }

    public void ButtonPressed(Button button)
    {
        button.transform.DOPunchScale(button.transform.localScale * 0.1f, 0.1f, 0, 0);
        audioSource.PlayOneShot(buttonClip);
    }

    private void OnDisable()
    {
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            button.onClick.RemoveListener(() => ButtonPressed(button));
        }
    }
}
