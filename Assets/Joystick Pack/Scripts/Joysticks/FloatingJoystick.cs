using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloatingJoystick : Joystick
{
    private GameObject joystick;

    protected override void Start()
    {
        joystick = GameObject.Find("Fixed Joystick");
        background.GetComponent<Image>().enabled = false;
        handle.GetComponent<Image>().enabled = false;
        base.Start();
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            joystick.transform.position = Input.mousePosition;
            background.GetComponent<Image>().enabled = true;
            handle.GetComponent<Image>().enabled = true;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.GetComponent<Image>().enabled = false;
        handle.GetComponent<Image>().enabled = false;
        base.OnPointerUp(eventData);
    }
}