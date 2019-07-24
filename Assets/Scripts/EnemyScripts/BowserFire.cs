using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowserFire : MonoBehaviour
{

    private GameObject player;
    private Rigidbody2D rb;
    private readonly float fireSpeed = 1.5f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Destroy(gameObject, 3.0f);
    }

    void Update()
    {
        rb.velocity = transform.right * fireSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerMovement>().Hit();
        }
    }

}
