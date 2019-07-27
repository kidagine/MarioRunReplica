using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikeball : MonoBehaviour
{

    [SerializeField] private GameObject smokePrefab;
    
    private Rigidbody2D rb;
    private int rotationSpeed = 200;
    private float rollSpeed = -1.4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 3.0f);
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        rb.velocity = new Vector2(rollSpeed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Breakable"))
        {
            FindObjectOfType<AudioManager>().Play("BreakBlock");
            Instantiate(smokePrefab, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
    }

}
