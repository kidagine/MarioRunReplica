using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bubble : MonoBehaviour
{

    [HideInInspector] public bool hasReachedStart;
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
                player.transform.parent = null;
                Destroy(gameObject);
            }
        }
    }

    private void MoveBubble()
    {
        if (transform.position.x > - 5.5f)
        {
            rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
            hasReachedStart = true;
        }
    }

}
