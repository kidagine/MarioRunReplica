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
    private float runSpeed = 4.0f;
    private float topYPosition = 3.0f;
    private float bottomYPosition = 2.0f;
    private float targetYPosition = 3.0f;
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
            topYPosition = Random.Range(2.9f, 3.1f);
        }
        if (transform.position.y <= bottomYPosition + 0.2f && hasReachedTopOnce)
        {
            targetYPosition = topYPosition;
            bottomYPosition = Random.Range(1.9f, 2.1f);
        }
    }

    private void DestroyBubble()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.isBubbled = false;
                player.transform.parent = null;
                Destroy(gameObject);
            }
        }
    }

    private void MoveBubble()
    {
        if (transform.position.x > - 6.5f)
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
