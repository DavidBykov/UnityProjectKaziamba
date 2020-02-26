using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float smooth;
    public Transform player;
    public float verticalLenght;
    public float horizontalLenght;

    private Joystick joystick;

    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
    }

    void FixedUpdate()
    {
        Vector3 cameraPosition = new Vector3(horizontalLenght * joystick.Horizontal, transform.localPosition.y, verticalLenght * joystick.Vertical);

        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraPosition, Time.timeScale * smooth);
    }
}
