using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowser : MonoBehaviour
{

    [SerializeField] private TriggerCinematic triggerCinematic;
    [SerializeField] private Animator koopaClownCarAnimator;
    [SerializeField] private Animator smokeAnimator;
    [SerializeField] private ClockTimer clockTimer;
    [SerializeField] private GameObject bowserCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bowserSmoke;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private GameObject BombOmbPrefab;
    [SerializeField] private GameObject SpikeballPrefab;

    private Animator animator;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private Vector3 originPosition;
    private float runSpeed;
    private float offset = 3.1f;
    private float switchPositionCooldown = 5.0f;
    private float attackCooldown = 3.0f;
    private int lastChosenPosition;
    private int health = 3;
    private bool isKeepingDistance;
    private bool isChangingPosition;
    private bool isInvunrable;
    private bool isDead;
    private bool isAttacking;
    

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.isScrollingOn && triggerCinematic.isCameraSwitchOver && !isDead)
        {
            CheckForPlayerDistance();
            FlyRight();
            FloatFly();
            ChooseRandomPosition();
            ChooseRandomAttack();
        }
        else if (GameManager.isScrollingOn)
        {
            rb.velocity = Vector2.zero;
        }
        if (isDead)
        {
            float step = 1.2f * Time.deltaTime;
            transform.position = originPosition + Random.insideUnitSphere * 0.1f;
        }
    }


    private void LateUpdate()
    {
        if (isKeepingDistance)
        {
            float targetXPosition = player.transform.position.x + offset;
            transform.position = new Vector2(targetXPosition, transform.position.y);
        }
    }

    private void FlyRight()
    {
        rb.velocity = new Vector2(runSpeed, rb.velocity.y);
    }

    private void FloatFly()
    {
        if (!isChangingPosition && switchPositionCooldown >= 0.0f && isKeepingDistance && !isAttacking)
        {
            //originPosition.y += Mathf.Sin(Time.time * 1) * 0.01f;
            //transform.position = originPosition;
        }
    }

    private void CheckForPlayerDistance()
    {
        if (!isKeepingDistance)
        {
            float distance = Vector2.Distance(new Vector2(transform.position.x, 0.0f), new Vector2(player.transform.position.x, 0.0f));
            if (distance > offset)
            {
                isKeepingDistance = true;
            }
            else
            {
                runSpeed = 5.0f;
            }
        }
    }

    private void ChooseRandomPosition()
    {
        if (switchPositionCooldown < 0.0f && !isChangingPosition)
        {
            float positionYOne = -0.6f;
            float positionYTwo = 0.9f;
            float positionYThree = 2.4f;
            float positionYFour = 3.3f;
            int randomPosition = Random.Range(0, 4);
            if (lastChosenPosition != randomPosition)
            {
                FindObjectOfType<AudioManager>().Play("TroopaClownCarWoosh");
            }

            if (randomPosition == 0)
            {
                lastChosenPosition = 0;
                Vector2 targetPosition = new Vector2(transform.position.x, positionYOne);
                StartCoroutine(MoveToPosition(targetPosition));
            }
            else if (randomPosition == 1)
            {
                lastChosenPosition = 1;
                Vector2 targetPosition = new Vector2(transform.position.x, positionYTwo);
                StartCoroutine(MoveToPosition(targetPosition));
            }
            else if (randomPosition == 2)
            {
                lastChosenPosition = 2;
                Vector2 targetPosition = new Vector2(transform.position.x, positionYThree);
                StartCoroutine(MoveToPosition(targetPosition));
            }
            else
            {
                lastChosenPosition = 3;
                Vector2 targetPosition = new Vector2(transform.position.x, positionYFour);
                StartCoroutine(MoveToPosition(targetPosition));
            }
        }
        switchPositionCooldown -= Time.deltaTime;
    }

    IEnumerator MoveToPosition(Vector2 positionToMoveTo)
    {
        float ratio = 0.0f;
        isChangingPosition = true;
        while (isChangingPosition)
        {
            if (ratio <= 1.0f)
            {
                transform.position = Vector2.Lerp(transform.position, positionToMoveTo, ratio);
                ratio += 1.0f * Time.deltaTime;
                yield return null;
            }
            else
            {
                originPosition = transform.position;
                transform.position = positionToMoveTo;
                switchPositionCooldown = 5.0f;
                isChangingPosition = false;
            }
        }
    }

    private void ChooseRandomAttack()
    {
        if (attackCooldown < 0.0f)
        {
            int randomAttack = Random.Range(0, 3);
            if (randomAttack == 0)
            {
                attackCooldown = 3.0f;
                animator.SetBool("IsFireBreathing", true);
            }
            else if (randomAttack == 1)
            {
                attackCooldown = 3.0f;
                animator.SetBool("IsThrowingBombOmb", true);
            }
            else if (randomAttack == 2)
            {
                attackCooldown = 3.0f;
                koopaClownCarAnimator.SetBool("IsShooting", true);
            }
        }
        attackCooldown -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Bomb"))
        {
            BombOmb bombOmb = other.gameObject.GetComponent<BombOmb>();
            if (bombOmb.isHit && !isInvunrable)
            {
                clockTimer.ResetTimer();
                FindObjectOfType<AudioManager>().Play("Explosion");
                FindObjectOfType<AudioManager>().Play("BowserHit");
                isInvunrable = true;
                health--;
                Instantiate(smokePrefab, other.transform.position, Quaternion.identity);
                if (health <= 0)
                {
                    Debug.Log("tes");
                    Death();
                    originPosition = transform.position;
                }
                else
                {
                    animator.SetBool("IsHit", true);
                    koopaClownCarAnimator.SetBool("IsHit", true);
                    StartCoroutine(ResetInvunrability());
                }
                Destroy(other.gameObject);
            }
        }
    }

    private void Death()
    {
        isDead = true;
        isKeepingDistance = false;
        bowserSmoke.SetActive(true);
        bowserCamera.SetActive(true);
        animator.SetBool("IsDead", true);
        smokeAnimator.SetBool("IsBowserDead", true);
        koopaClownCarAnimator.SetBool("IsHit", true);
        GameManager.isScrollingOn = false;
        FindObjectOfType<AudioManager>().Play("BowserHit");
    }

    public void PlayAudio(string audioName)
    {
        FindObjectOfType<AudioManager>().Play(audioName);
    }

    public void InstantiateAttackPrefab(GameObject attackPrefab)
    {
        if (attackPrefab.name.Equals("BowserFire"))
        {
            Instantiate(attackPrefab, new Vector2(transform.position.x - 0.26f, transform.position.y + 0.16f), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("BowserFire");
            animator.SetBool("IsFireBreathing", false);
        }
        else if (attackPrefab.name.Equals("Bombomb"))
        {
            Instantiate(attackPrefab, new Vector2(transform.position.x - 0.1f, transform.position.y + 0.25f), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Throw");
            animator.SetBool("IsThrowingBombOmb", false);
        }
        else if (attackPrefab.name.Equals("Spikeball"))
        {
            Instantiate(smokePrefab, new Vector2(transform.position.x - 0.15f, transform.position.y - 0.3f), Quaternion.identity);
            Instantiate(attackPrefab, new Vector2(transform.position.x, transform.position.y - 1.0f), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Explosion");
            koopaClownCarAnimator.SetBool("IsShooting", false);
        }
        isAttacking = false;
    }

    IEnumerator ResetInvunrability()
    {
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("IsHit", false);
        koopaClownCarAnimator.SetBool("IsHit", false);
        isInvunrable = false;
        yield return null;
    }

    public void DeathLaunch()
    {
        FindObjectOfType<AudioManager>().Play("BowserDeath");
        bowserCamera.SetActive(false);
        animator.SetBool("IsDead", false);
        smokeAnimator.SetBool("IsBowserDead", false);
        koopaClownCarAnimator.SetBool("IsHit", false);
        StartCoroutine(Launch());
        StartCoroutine(Rotate());
    }

    IEnumerator Launch()
    {
        yield return new WaitForSeconds(0.2f);
        float ratio = 0.0f;
        Vector2 startingPoint = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPoint = new Vector2(transform.position.x - 3.0f, transform.position.y - 9f);
        Vector2 controlPoint = startingPoint + (targetPoint - startingPoint) / 2 + Vector2.up * 5.0f;
        while (ratio < 1.0f)
        {
            Vector3 m1 = Vector3.Lerp(startingPoint, controlPoint, ratio);
            Vector3 m2 = Vector3.Lerp(controlPoint, targetPoint, ratio);
            transform.position = Vector3.Lerp(m1, m2, ratio);
            ratio += 0.5f * Time.deltaTime;
            if (ratio >= 1.0f)
            {
                FindObjectOfType<GameManager>().PlayerWon();
                Destroy(gameObject);
            }
            yield return null;
        }
    }

    IEnumerator Rotate()
    {
        while (isDead)
        {
            transform.Rotate(0, 0, 15);
            yield return null;
        }
    }

}
