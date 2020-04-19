using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonScaleAnimation : MonoBehaviour
{
    private void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.DOPunchScale(rectTransform.localScale * 0.1f, 0.7f, 0, 1).SetLoops(-1);
    }
}
