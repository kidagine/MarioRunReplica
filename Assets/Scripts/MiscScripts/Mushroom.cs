using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{

    [SerializeField] SpriteRenderer spriteRenderer;

    private Vector2 startingPoint;
    private Vector2 targetPoint;
    private Vector2 controlPoint;
    private bool isFacingRight;
    private float ratio;
    private float runSpeed = 2.0f;


    void Start()
    {
        startingPoint = new Vector2(transform.position.x, transform.position.y);
        targetPoint = new Vector2(transform.position.x + 2.5f, transform.position.y - 1f);
        controlPoint = startingPoint + (targetPoint - startingPoint) / 2 + Vector2.up * 2.0f;
    }

    void Update()
    {
        if (ratio < 1.0f)
        {
            LaunchItem();
        }
        else
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

    private void LaunchItem()
    {
        ratio += 1.2f * Time.deltaTime;

        Vector3 m1 = Vector3.Lerp(startingPoint, controlPoint, ratio);
        Vector3 m2 = Vector3.Lerp(controlPoint, targetPoint, ratio);
        transform.position = Vector3.Lerp(m1, m2, ratio);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().Play("PowerUp");
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            isFacingRight = !isFacingRight;
        }
    }

}
