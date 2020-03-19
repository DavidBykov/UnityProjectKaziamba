using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Animator animator;

    [HideInInspector] public float _speedWithoutSouls;
    [HideInInspector] public float _speedWithSouls;
    private float _curentSpeed;
    private Joystick joystick;

    [HideInInspector] public int curentSoulsTargeting;

    void OnEnable()
    {
        _speedWithoutSouls = FindObjectOfType<GameSettings>().GetGameParemeters().playerSpeed;
        _speedWithSouls = FindObjectOfType<GameSettings>().GetGameParemeters().playerSpeedWhenTargetingSouls;
        _curentSpeed = _speedWithoutSouls;
        joystick = FindObjectOfType<Joystick>();
    }

    void FixedUpdate()
    {
        if (curentSoulsTargeting > 0)
            _curentSpeed = _speedWithSouls;
        else
            _curentSpeed = _speedWithoutSouls;

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(transform.forward * _curentSpeed * joystick.Vertical);
        //rigidbody.AddForce(transform.forward * _speed * -1f);
        rigidbody.AddForce(transform.right * _curentSpeed * joystick.Horizontal);

        if(joystick.Vertical != 0f || joystick.Horizontal != 0f) animator.SetBool("Moving", true); else animator.SetBool("Moving", false);
    }
}
