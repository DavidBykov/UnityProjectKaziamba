using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonController : MonoBehaviour
{
    public float speed;
    public Vector3 targetMove;
    private SpriteRenderer rend;

    Rigidbody2D body;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rend.sortingOrder = 30000 + (int)(transform.position.y * -1000);

        body.velocity = new Vector2(targetMove.x * speed, targetMove.y * speed);

        //Vector2 heading = new Vector2(targetMove.x, targetMove.y).normalized;
        //body.AddForce(heading * speed);
    }
}
