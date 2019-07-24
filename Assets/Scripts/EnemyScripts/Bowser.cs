using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowser : MonoBehaviour
{

    [SerializeField] private TriggerCinematic triggerCinematic;
    [SerializeField] private Animator koopaClownCarAnimator;
    [SerializeField] private ClockTimer clockTimer;
    [SerializeField] private GameObject bowserCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private GameObject BombOmbPrefab;
    [SerializeField] private GameObject SpikeballPrefab;

    private Animator animator;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private float runSpeed;
    private float offset = 3.1f;
    private float switchPositionCooldown = 5.0f;
    private float attackCooldown = 3.0f;
    private int lastChosenPosition;
    private int health = 3;
    private bool isKeepingDistance;
    private bool isChangingPosition;
    private bool isInvunrable;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.isScrollingOn && triggerCinematic.isCameraSwitchOver)
        {
            CheckForPlayerDistance();
            FlyRight();
            FloatFly();
            ChooseRandomPosition();
            ChooseRandomAttack();
            if (health <= 0)
            {
                Death();
            }
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
        //if (!isChangingPosition  && switchPositionCooldown >= 0.0f)
        //{
        //    transform.position = transform.right * Mathf.Sin(Time.deltaTime * 1) * 0.2f;
        //}
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
            float positionYOne = -0.2f;
            float positionYTwo = 1.3f;
            float positionYThree = 2.8f;
            float positionYFour = 4.3f;
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
                animator.SetBool("IsHit", true);
                koopaClownCarAnimator.SetBool("IsHit", true);
                isInvunrable = true;
                health--;
                Instantiate(smokePrefab, other.transform.position, Quaternion.identity);
                StartCoroutine(ResetInvunrability());
                Destroy(other.gameObject);
            }
        }
    }

    private void Death()
    {
        bowserCamera.SetActive(true);
        animator.SetBool("IsDead", true);
        FindObjectOfType<AudioManager>().Play("BowserHit");
        Debug.Log("dead");
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
            Instantiate(attackPrefab, new Vector2(transform.position.x - 0.15f, transform.position.y + 0.26f), Quaternion.identity);
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
    }

    IEnumerator ResetInvunrability()
    {
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("IsHit", false);
        koopaClownCarAnimator.SetBool("IsHit", false);
        isInvunrable = false;
        yield return null;
    }

}
