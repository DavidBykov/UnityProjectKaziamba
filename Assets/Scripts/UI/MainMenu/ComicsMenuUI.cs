using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ComicsMenuUI : MonoBehaviour
{
    private List<Sprite> comics = new List<Sprite>();
    private Sprite curentComic;
    public Image image;

    public void SetComic(List<Sprite> _comics, Sprite _curentComic)
    {
        comics = _comics;
        curentComic = _curentComic;
        image.sprite = curentComic;
    }

    public void NextComic()
    {

    }

    public void PrevComic()
    {

    }
}
