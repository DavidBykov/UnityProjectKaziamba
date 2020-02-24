using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    private bool moving;
    private bool alive = true;
    public float speed;

    public Rigidbody rigidbody;
    public Animator animator;

    public GameObject scaryFace;
    public GameObject idleFace;
    public GameObject dieFace;

    private Player player;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && alive)
        {
            moving = true;
            animator.SetBool("Scary", true);
            idleFace.SetActive(false);
            scaryFace.SetActive(true);
        }
        Debug.Log(other.name);
        if(other.tag == "Fire" && alive)
        {
            GamePlay.instance.AddSouls();
            alive = false;
            dieFace.SetActive(true);
            scaryFace.SetActive(false);
            idleFace.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && alive)
        {
            moving = false;
            animator.SetBool("Scary", false);
            idleFace.SetActive(true);
            scaryFace.SetActive(false);
        }
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (moving && alive)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce((transform.position - player.transform.position) * speed);
        }
    }
}
