using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaTroopa : MonoBehaviour, IEnemy
{

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject ImpactEffectPrefab;
    [SerializeField] private bool isFacingRight;

    private bool isSpinning;
    private bool isInsideMainCamera;
    private bool canMove = true;
    private float runSpeed = 0.6f;


    void Update()
    {
        Walk();
    }

    public void Walk()
    {
        if (canMove && isInsideMainCamera)
        {
            if (!isFacingRight)
            {
                transform.Translate(Vector2.right * runSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.right * -runSpeed * Time.deltaTime);
            }
        }
    }

    public void Stomped()
    {
        if (!isSpinning)
        {
            animator.SetTrigger("Stomped");
            Instantiate(ImpactEffectPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
            canMove = false;
            transform.Translate(Vector2.right * 0.0f);
        }
    }

    public void Spin()
    {
        animator.SetTrigger("Spin");
        canMove = true;
        isSpinning = true;
        runSpeed = runSpeed * 4f;
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
        else if (other.gameObject.CompareTag("Enemy"))
        {
            if (isSpinning)
            {
                FindObjectOfType<AudioManager>().Play("Stomp");
                other.gameObject.GetComponent<Goomba>().Hit();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("MainCamera"))
        {
            isInsideMainCamera = true;
        }
    }

}
