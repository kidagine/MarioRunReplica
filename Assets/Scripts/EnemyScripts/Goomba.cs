using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{

    [SerializeField] Animator animator;

    private bool isFacingRight;
    private bool canMove = true;
    private float runSpeed = 0.8f;


    void Update()
    {
        Walk();
    }

    private void Walk()
    {
        if (canMove)
        {
            if (isFacingRight)
            {
                transform.Translate(Vector2.right * runSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.right * -runSpeed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
            isFacingRight = !isFacingRight;
        }
    }

    public void Stomped()
    {
        animator.SetTrigger("Stomped");
        canMove = false;
        transform.Translate(Vector2.right * 0.0f);
        Destroy(gameObject, 0.1f);
    }

}
