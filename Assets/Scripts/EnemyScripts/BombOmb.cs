using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombOmb : MonoBehaviour, IEnemy
{

    [SerializeField] private GameObject impactEffectPrefab;
    [HideInInspector] public bool isHit;

    private GameObject player;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 startingPoint;
    private Vector2 targetPoint;
    private Vector2 controlPoint;   
    private float ratio;
    private float runSpeed = 0.6f;
    private float launchForce = 13.0f;
    private bool canMove = true;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startingPoint = new Vector2(transform.position.x, transform.position.y);
        targetPoint = new Vector2(transform.position.x - 0.05f, transform.position.y);
        controlPoint = startingPoint + (targetPoint - startingPoint) / 2 + Vector2.up * 1.5f;
        player = GameObject.FindGameObjectWithTag("Player");
        Destroy(gameObject, 3.0f);
    }   

    void Update()
    {
        if (ratio < 1.0f)
        {
            LaunchItem();
        }
        else
        {
            Walk();
        }
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

    private void CheckForPlayerDistance()
    {
        float distance = Vector2.Distance(new Vector2(transform.position.x, 0.0f), new Vector2(player.transform.position.x, 0.0f));
        if (distance > 3.8f)
        {
            Destroy(gameObject);
        }
    }

    private void LaunchItem()
    {
        ratio += 1.5f * Time.deltaTime;

        Vector3 m1 = Vector3.Lerp(startingPoint, controlPoint, ratio);
        Vector3 m2 = Vector3.Lerp(controlPoint, targetPoint, ratio);
        transform.position = Vector3.Lerp(m1, m2, ratio);
    }

    public void Stomped(int killStreak)
    {
        FindObjectOfType<AudioManager>().Play("Stomp");
        animator.SetTrigger("Stomped");
        Instantiate(impactEffectPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
        canMove = false;
        isHit = true;
        rb.velocity = new Vector2(launchForce, 3.0f);
    }

    public void Hit(int killstreak)
    {
        throw new System.NotImplementedException();
    }

}
