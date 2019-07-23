using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowserFire : MonoBehaviour
{

    private Rigidbody2D rb;
    private readonly float fireSpeed = 1.5f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 3.0f);
    }

    void Update()
    {
        rb.velocity = new Vector2(-fireSpeed, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerMovement>().Hit();
        }
    }

}
