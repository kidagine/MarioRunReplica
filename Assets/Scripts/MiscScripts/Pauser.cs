using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pauser : MonoBehaviour
{

    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("IsPressedDown", true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("IsPressedDown", false);
        }
    }

}
