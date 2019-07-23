using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombOmb : MonoBehaviour, IEnemy
{

    [SerializeField] private GameObject impactEffectPrefab;
    [HideInInspector] public bool isHit;

    private Animator animator;
    private Rigidbody2D rb;
    private float runSpeed = 0.7f;
    private float launchForce = 13.0f;
    private bool canMove = true;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 3.0f);
    }

    void Update()
    {
        Walk();
    }

    public void Walk()
    {
        if (canMove)
        {
            rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
            if (rb.velocity.y < -0.1f)
            {
                animator.SetBool("IsFalling", true);
            }
            else
            {
                animator.SetBool("IsFalling", false);
            }
        }
    }

    public void Stomped(int killStreak)
    {
        FindObjectOfType<AudioManager>().Play("Stomp");
        animator.SetTrigger("Stomped");
        Instantiate(impactEffectPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
        canMove = false;
        isHit = true;
        rb.velocity = new Vector2(launchForce, 0.0f);
    }

    public void Hit(int killstreak)
    {
        throw new System.NotImplementedException();
    }

}
