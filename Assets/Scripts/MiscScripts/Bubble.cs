using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bubble : MonoBehaviour
{

    [HideInInspector] public bool hasReachedStart;
    private GameObject boundary;
    private GameObject introPanel;
    private GameObject player;
    private Rigidbody2D rb;
    private Vector3 velocity = new Vector3(1.0f, 1.0f, 0.0f);
    private Vector2 bubblePosition;
    private float runSpeed = 3.5f;
    private float topYPosition = 2.5f;
    private float bottomYPosition = 1.5f;
    private float targetYPosition = 2.5f;
    private bool hasReachedTopOnce;

    
    void Start()
    {
        boundary = GameObject.Find("BubbleBoundary");
        player = GameObject.Find("Player");
        introPanel = GameObject.Find("BlackPanel");
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.InsideBubble(gameObject);
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        DestroyBubble();
        MoveBubble();
    }

    private void LateUpdate()
    {
        PlayerFollowBubble();
        transform.position = Vector3.SmoothDamp(transform.position, new Vector2(transform.position.x, targetYPosition), ref velocity, 1.0f);
        if (transform.position.y >= topYPosition - 0.2f)
        {
            hasReachedTopOnce = true;
            targetYPosition = bottomYPosition;
            topYPosition = Random.Range(2.4f, 2.6f);
        }
        if (transform.position.y <= bottomYPosition + 0.2f && hasReachedTopOnce)
        {
            targetYPosition = topYPosition;
            bottomYPosition = Random.Range(1.4f, 1.6f);
        }
    }

    private void DestroyBubble()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0) && transform.position.y > bottomYPosition)
            {
                GameManager.isBubbled = false;
                FindObjectOfType<GameManager>().DecrementBubbles(1);
                player.GetComponent<BoxCollider2D>().enabled = true;
                player.GetComponent<CircleCollider2D>().enabled = true;
                Destroy(gameObject);
            }
        }
    }

    private void MoveBubble()
    {
        if (transform.position.x > boundary.transform.position.x)
        {
            rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
            hasReachedStart = true;
        }
    }

    private void PlayerFollowBubble()
    {
        player.transform.position = transform.position;
    }

}
