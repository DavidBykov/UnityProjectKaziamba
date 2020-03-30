using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public Animator animator;
    public void PlayCatchedEffect()
    {
        animator.SetTrigger("Catched");
    }
}
