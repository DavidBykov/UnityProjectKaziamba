using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Animator animator;

    [HideInInspector] public float _speed;
    private Joystick joystick;
    
    void OnEnable()
    {
        _speed = FindObjectOfType<GameSettings>().GetGameParemeters().playerSpeed;
        joystick = FindObjectOfType<Joystick>();
    }

    void FixedUpdate()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(transform.forward * _speed * joystick.Vertical);
        //rigidbody.AddForce(transform.forward * _speed * -1f);
        rigidbody.AddForce(transform.right * _speed * joystick.Horizontal);

        if(joystick.Vertical != 0f || joystick.Horizontal != 0f) animator.SetBool("Moving", true); else animator.SetBool("Moving", false);
    }
}
