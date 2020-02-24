using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Animator animator;

    public float speed;
    private Joystick joystick;
    
    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
    }

    void FixedUpdate()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(transform.forward * speed * joystick.Vertical);
        rigidbody.AddForce(transform.right * speed * joystick.Horizontal);

        if(joystick.Vertical != 0f || joystick.Horizontal != 0f) animator.SetBool("Moving", true); else animator.SetBool("Moving", false);
    }
}
