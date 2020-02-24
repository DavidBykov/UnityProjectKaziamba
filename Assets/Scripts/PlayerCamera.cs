using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float smooth;
    public Transform player;

    void Update()
    {
        transform.position =  Vector3.Lerp(transform.position, player.position, Time.timeScale * smooth);
    }
}
