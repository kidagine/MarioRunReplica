using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour, IEnemy
{

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject ImpactEffectPrefab;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isFacingRight = false;
    private bool isInsideMainCamera;
    private bool canMove = true;
    private float runSpeed = 0.6f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Walk();
    }

    public void Walk()
    {
        if (canMove && isInsideMainCamera)
        {
            if (isFacingRight)
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
        animator.SetTrigger("Stomped");
        Instantiate(ImpactEffectPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
        canMove = false;
        transform.Translate(Vector2.right * 0.0f);
        Destroy(gameObject, 0.15f);
    }

    public void Hit()
    {
        Instantiate(ImpactEffectPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        rb.gravityScale = 0.0f;
        boxCollider.enabled = false;
        Vector2 startingPoint = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPoint = new Vector2(transform.position.x + 2.5f, transform.position.y - 2f);
        Vector2 controlPoint = startingPoint + (targetPoint - startingPoint) / 2 + Vector2.up * 5.0f;
        StartCoroutine(Launch(startingPoint, targetPoint, controlPoint));
    }

    IEnumerator Launch(Vector2 startingPoint, Vector2 targetPoint, Vector2 controlPoint)
    {
        bool isDestroyed = false;
        float ratio = 0.0f;
        float rotationValue = 0.0f;
        float timeUntilDestroy = 2.0f;

        while (!isDestroyed)
        {
            if (timeUntilDestroy >= 0.0f)
            {
                Vector3 m1 = Vector3.Lerp(startingPoint, controlPoint, ratio);
                Vector3 m2 = Vector3.Lerp(controlPoint, targetPoint, ratio);
                transform.position = Vector3.Lerp(m1, m2, ratio);

                transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, rotationValue));

                timeUntilDestroy -= Time.deltaTime;
                rotationValue -= 35 * Time.deltaTime;
                ratio += 1.0f * Time.deltaTime;
                yield return null;
            }
            else
            {
                isDestroyed = true;
                Destroy(gameObject);
            }
        }
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("MainCamera"))
        {
            isInsideMainCamera = true;
        }
    }

}
