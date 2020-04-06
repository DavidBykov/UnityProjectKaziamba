using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRenderer : MonoBehaviour
{
    private SpriteRenderer rend;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        rend.sortingOrder = 30000 + (int)(transform.position.y * -1000);
    }
}
