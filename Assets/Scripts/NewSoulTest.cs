using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSoulTest : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Animator animator;
    public List<NewSoulTest> souls;
    public Collider enemyCollider;
    public float vectorLenght;
    public float distanceToPlayerStartWeight;
    public float speed;

    private Player player;
    public bool alarm;
    private bool isDead;

    private Vector3 finalDestination;
    private Vector3 soulsVectorsSum;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Soul" && !isDead)
        {
            Debug.Log(other.name);
            souls.Add(other.GetComponentInParent<NewSoulTest>());
        }

        if (other.tag == "Player" && !isDead)
        {
            animator.SetBool("Scary", true);
            alarm = true;
        }

        if (other.tag == "Fire" && !isDead)
        {
            isDead = true;
            GamePlay.instance.AddSouls();
            animator.SetBool("Scary", false);
            enemyCollider.enabled = false;
            rigidbody.isKinematic = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Soul" && !isDead)
        {
            souls.Remove(other.GetComponentInParent<NewSoulTest>());
        }

        if (other.tag == "Player" && !isDead)
        {
            animator.SetBool("Scary", false);
            alarm = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            finalDestination = Vector3.Normalize(transform.position - player.transform.position);
            soulsVectorsSum = Vector3.zero;

            foreach (NewSoulTest soul in souls)
            {
                if (soul.GetComponent<NewSoulTest>().alarm)
                    soulsVectorsSum += (soul.transform.position - player.transform.position);
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer > distanceToPlayerStartWeight)
            {
                finalDestination = soulsVectorsSum;
            }
            else
            {
                float percent = distanceToPlayer / distanceToPlayerStartWeight;

                finalDestination = (transform.position - player.transform.position) + soulsVectorsSum * Mathf.Pow(percent, 3);
            }

            if (alarm)
            {
                rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f);
                rigidbody.AddForce(new Vector3(finalDestination.normalized.x, 0f, finalDestination.normalized.z) * speed);
            }

            Color directionColor;
            if (alarm)
                directionColor = Color.red;
            else
                directionColor = Color.green;

            DrawDirectionInfo(finalDestination, directionColor);
        }
    }

    private void DrawDirectionInfo(Vector3 direction, Color color)
    {
        Debug.DrawRay(transform.position, Vector3.Normalize(transform.position - player.transform.position) * vectorLenght, Color.yellow);
        Debug.DrawRay(transform.position, direction * vectorLenght, color);
    }
}
