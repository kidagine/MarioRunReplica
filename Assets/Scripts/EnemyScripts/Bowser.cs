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
    [SerializeField] private GameObject bombOmbPrefab;
    [SerializeField] private GameObject spikeballPrefab;
    [SerializeField] private GameObject bigSpikeballPrefab;

    private Animator animator;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private Vector3 originPosition;
    private Vector3 floatPosition;
    private List<int> attackPattern = new List<int>();
    private float runSpeed = 5.0f;
    private float offset = 3.1f;
    private float switchPositionCooldown = 3.0f;
    private float timeToReachPlayer;
    private int lastChosenPosition;
    private int lastChosenAttack;
    private int health = 3;
    private bool isKeepingDistance;
    private bool isChangingPosition;
    private bool isInvunrable;
    private bool isDead;
    private bool isAttacking;
    private bool isFarFromPlayer;
    private bool hasMovedOnce;

    //Attack Pattern variables
    private bool wasHit;
    private bool isRaged;
    private bool hasAttackedOnce;
    private bool hasOneHealth;
    private bool hasThrownBigSpikeballOnce;
    private bool hasReachedEndOfAttackPattern;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.isScrollingOn && triggerCinematic.isCameraSwitchOver && !isFarFromPlayer && !isDead)
        {
            CheckForPlayerDistance();
            ChooseRandomAttack();
            ChooseRandomPosition();
            FloatFly();
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

        if (GameManager.isBubbled)
        {
            isFarFromPlayer = true;
        }
        if (!GameManager.isBubbled && isFarFromPlayer)
        {
            ReachPlayer();
        }
    }


    private void LateUpdate()
    {
        if (!isFarFromPlayer && isKeepingDistance)
        {
            float targetXPosition = player.transform.position.x + offset;
            transform.position = new Vector2(targetXPosition, transform.position.y);
        }
    }

    private void FloatFly()
    {
        if (isKeepingDistance)
        {
            floatPosition = transform.position;
            floatPosition.y += Mathf.Sin(Time.time * 2.0f) * 0.005f;
            transform.position = floatPosition;
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
                rb.velocity = new Vector2(runSpeed, rb.velocity.y);
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
            int randomPosition = 0;
            FindObjectOfType<AudioManager>().Play("TroopaClownCarWoosh");
            if (!hasMovedOnce)
            {
                hasMovedOnce = true;
                int chooseRandomPosition = Random.Range(0, 6);
                if (chooseRandomPosition >= 1)
                {
                    randomPosition = 0;
                }
                else if (chooseRandomPosition >= 3 && chooseRandomPosition < 1)
                {
                    randomPosition = 2;
                }
                else
                {
                    randomPosition = 3;
                }
            }
            else if (lastChosenPosition == 0)
            {
                randomPosition = 1;
            }
            else if (lastChosenPosition == 1)
            {
                int chooseRandomPosition = Random.Range(0, 11);
                if (chooseRandomPosition > 8)
                {
                    randomPosition = 0;
                }
                else
                {
                    randomPosition = 2;
                }
            }
            else if (lastChosenPosition == 2)
            {
                int chooseRandomPosition = Random.Range(0, 11);
                if (chooseRandomPosition > 8)
                {
                    randomPosition = 1;
                }
                else
                {
                    randomPosition = 3;
                }
            }
            else if (lastChosenPosition == 3)
            {
                randomPosition = 2;
            }
            else
            {
                randomPosition = Random.Range(0, 4);
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
            else if (randomPosition == 3)
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
                ratio += Time.deltaTime;
                yield return null;
            }
            else
            {
                originPosition = transform.position;
                transform.position = positionToMoveTo;
                switchPositionCooldown = 3.0f;
                isChangingPosition = false;
            }
        }
    }

    private void ChooseRandomAttack()
    {
        if (!hasReachedEndOfAttackPattern)
        {
            if (wasHit)
            {
                wasHit = false;
                isRaged = true;
                attackPattern = new List<int> { 0, 0, 0 };
            }
            else if (!hasAttackedOnce)
            {
                int randomAttackPattern = Random.Range(0, 2);
                if (randomAttackPattern == 0)
                {
                    attackPattern = new List<int> { 1, 1, 1 };
                }
                if (randomAttackPattern == 1)
                {
                    attackPattern = new List<int> { 2, 1, 1 };
                }
                hasAttackedOnce = true;
            }
            else if (hasOneHealth)
            {
                int randomAttackPattern = Random.Range(0, 2);
                if (!hasThrownBigSpikeballOnce)
                {
                    if (randomAttackPattern == 0)
                    {
                        attackPattern = new List<int> { 2, 1, 1 };
                    }
                    if (randomAttackPattern == 1)
                    {
                        attackPattern = new List<int> { 2, 1, 2 };
                    }
                }
                else
                {
                    if (randomAttackPattern == 0)
                    {
                        attackPattern = new List<int> { 0, 2, 1 };
                    }
                    if (randomAttackPattern == 1)
                    {
                        attackPattern = new List<int> { 0, 1, 1 };
                    }
                }
                hasOneHealth = false;
            }
            else
            {
                int randomAttackPattern = Random.Range(0, 9);
                if (randomAttackPattern == lastChosenAttack)
                {
                    return;
                }
                if (randomAttackPattern == 0)
                {
                    attackPattern = new List<int> { 0, 2, 1 };
                }
                if (randomAttackPattern == 1)
                {
                    attackPattern = new List<int> { 0, 1, 1 };
                }
                if (randomAttackPattern == 2)
                {
                    attackPattern = new List<int> { 1, 1, 0 };
                }
                if (randomAttackPattern == 3)
                {
                    attackPattern = new List<int> { 1, 1, 1 };
                }
                if (randomAttackPattern == 4)
                {
                    attackPattern = new List<int> { 1, 1, 2 };
                }
                if (randomAttackPattern == 5)
                {
                    attackPattern = new List<int> { 2, 0, 1 };
                }
                if (randomAttackPattern == 6)
                {
                    attackPattern = new List<int> { 2, 0, 2 };
                }
                if (randomAttackPattern == 7)
                {
                    attackPattern = new List<int> { 2, 1, 1 };
                }
                if (randomAttackPattern == 8)
                {
                    attackPattern = new List<int> { 2, 1, 2 };
                }
                lastChosenAttack = randomAttackPattern; 
            }
            hasReachedEndOfAttackPattern = true;
            StartCoroutine(AttackWithPattern(attackPattern));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Bomb"))
        {
            BombOmb bombOmb = other.gameObject.GetComponent<BombOmb>();
            if (bombOmb.isHit && !isInvunrable)
            {
                clockTimer.ResetTimer();
                wasHit = true;
                FindObjectOfType<AudioManager>().Play("Explosion");
                FindObjectOfType<AudioManager>().Play("BowserHit");
                isInvunrable = true;
                health--;
                Instantiate(smokePrefab, other.transform.position, Quaternion.identity);
                if (health <= 0)
                {
                    Death();
                    originPosition = transform.position;
                }
                else if (health == 1)
                {
                    hasOneHealth = true;
                    animator.SetBool("IsHit", true);
                    koopaClownCarAnimator.SetBool("IsHit", true);
                    StartCoroutine(ResetInvunrability());
                }
                else
                {
                    hasAttackedOnce = false;
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
            Instantiate(firePrefab, new Vector2(transform.position.x - 0.26f, transform.position.y + 0.16f), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("BowserFire");
            animator.SetBool("IsFireBreathing", false);
        }
        else if (attackPrefab.name.Equals("Bombomb"))
        {
            Instantiate(bombOmbPrefab, new Vector2(transform.position.x - 0.1f, transform.position.y + 0.25f), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Throw");
            animator.SetBool("IsThrowingBombOmb", false);
        }
        else if (attackPrefab.name.Equals("Spikeball"))
        {
            Instantiate(smokePrefab, new Vector2(transform.position.x - 0.15f, transform.position.y - 0.3f), Quaternion.identity);
            if (health == 1)
            {
                Instantiate(bigSpikeballPrefab, new Vector2(transform.position.x, transform.position.y - 1.0f), Quaternion.identity);
            }
            else
            {
                Instantiate(spikeballPrefab, new Vector2(transform.position.x, transform.position.y - 1.0f), Quaternion.identity);
            }
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

    private void ReachPlayer()
    {
        timeToReachPlayer += Time.deltaTime / 1f;
        transform.position = Vector3.Lerp(transform.position, new Vector2(player.transform.position.x + offset, transform.position.y), timeToReachPlayer);
        if (Mathf.Approximately(transform.position.x, player.transform.position.x + offset))
        {
            timeToReachPlayer = 0.0f;
            isFarFromPlayer = false;
        }
    }

    IEnumerator AttackWithPattern(List<int> attackPattern)
    {
        foreach (var i in attackPattern)
        {
            if (wasHit)
            {
                yield return null;
            }
            else
            {
                if (i == 0)
                {
                    animator.SetBool("IsFireBreathing", true);
                    yield return new WaitForSeconds(3.8f);
                }
                else if (i == 1)
                {
                    animator.SetBool("IsThrowingBombOmb", true);
                    yield return new WaitForSeconds(3.8f);
                }
                else if (i == 2)
                {
                    koopaClownCarAnimator.SetBool("IsShooting", true);
                    yield return new WaitForSeconds(3.8f);
                }
            }
        }
        if (isRaged)
        {
            isRaged = false;
        }
        hasReachedEndOfAttackPattern = false;        
        yield return null;
    }
}
