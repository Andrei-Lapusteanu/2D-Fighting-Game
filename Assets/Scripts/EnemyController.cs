using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    const int STATE_IDLE = 0;
    const int STATE_WALK = 1;
    const int STATE_ATTACK = 2;

    const float RAND_IDLE_TIME_MIN = 0.5f;
    const float RAND_IDLE_TIME_MAX = 1f;
    const float RAND_WALK_TIME_MIN = 1.25f;
    const float RAND_WALK_TIME_MAX = 2.25f;
    const float RAND_ATTACK_TIME_MIN = 0.25f;
    const float RAND_ATTACK_TIME_MAX = 2.1f;

    const int DEFAULT_HP = 40;
    const int DEFAULT_DMG = 5;
    const float MOVEMENT_FORCE = 1.0f;
    const float MAX_MOVE_VELOCITY = -1.0f;
    const string playerProjTag = "PlayerProjectile";

    private bool isFirstState = true;
    private float fireRate = 2.0f;
    private float nextFire = 0.0f;
    private int currentState;
    private int HP = DEFAULT_HP;
    private int projectileDamage = DEFAULT_DMG;
    public MoveDirection currentDirection = MoveDirection.Right;
    private Animator animator;

    private Rigidbody2D rb2d;
    public Rigidbody2D projectileLeft;
    public Rigidbody2D projectileRight;

    public enum MoveDirection
    {
        Left,
        Right
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.freezeRotation = true;

        StartCoroutine(BasicAI());
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
            Destroy(gameObject);

        CheckPlayerLocation();

        if (currentState == STATE_WALK && currentDirection == MoveDirection.Left)
        {
            if (rb2d.velocity.x > MAX_MOVE_VELOCITY)
                rb2d.AddForce(Vector2.left * MOVEMENT_FORCE, ForceMode2D.Impulse);
        }
        else if (currentState == STATE_WALK && currentDirection == MoveDirection.Right)
        {
            if (rb2d.velocity.x < Mathf.Abs(MAX_MOVE_VELOCITY))
                rb2d.AddForce(Vector2.right * MOVEMENT_FORCE, ForceMode2D.Impulse);
        }

        if (currentState == STATE_ATTACK && currentDirection == MoveDirection.Left)
        {
            if(Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                var proj = Instantiate(projectileLeft, transform.position + new Vector3(0, GetRandomYLocation(), 0), Quaternion.identity);
                proj.transform.localScale = new Vector3(4.5f, 4.5f);
            }
        }
        else if (currentState == STATE_ATTACK && currentDirection == MoveDirection.Right)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                var proj = Instantiate(projectileRight, transform.position + new Vector3(0, GetRandomYLocation(), 0), Quaternion.identity);
                proj.transform.localScale = new Vector3(4.5f, 4.5f);
            }
        }
    }

    private float GetRandomYLocation()
    {
        var randomNo = Random.Range(0, 2);

        if (randomNo == 0)
            return -1.3f;
        else return 1.3f;
    }

    private float waitBetweenActions = 0.1f;
    public IEnumerator BasicAI()
    {
        if(isFirstState)
        {
            StartCoroutine(PerformWalkTask());
            isFirstState = false;
        }

        for (; ; )
        {
            yield return new WaitForSeconds(waitBetweenActions);

            var action = Random.Range(0, 3);

            switch (action)
            {
                case STATE_IDLE:
                    StartCoroutine(PerformIdleTask());
                    break;

                case STATE_WALK:
                    StartCoroutine(PerformWalkTask());
                    break;

                case STATE_ATTACK:
                    StartCoroutine(PerformAttackTask());
                    break;
            }
        }
    }

    private IEnumerator PerformIdleTask()
    {
        currentState = STATE_IDLE;
        animator.SetInteger("enemyState", currentState = STATE_IDLE);
        waitBetweenActions = Random.Range(RAND_IDLE_TIME_MIN, RAND_IDLE_TIME_MAX);

        //Debug.Log("Performing Idle task " + waitBetweenActions);

        yield return new WaitForSeconds(waitBetweenActions);
    }

    private IEnumerator PerformWalkTask()
    {
        currentState = STATE_WALK;
        animator.SetInteger("enemyState", currentState = STATE_WALK);
        waitBetweenActions = Random.Range(RAND_WALK_TIME_MIN, RAND_WALK_TIME_MAX);

        //Debug.Log("Performing walk task " + waitBetweenActions);

        yield return new WaitForSeconds(waitBetweenActions);
    }

    private IEnumerator PerformAttackTask()
    {
        currentState = STATE_ATTACK;
        animator.SetInteger("enemyState", currentState = STATE_ATTACK);
        waitBetweenActions = Random.Range(RAND_ATTACK_TIME_MIN, RAND_ATTACK_TIME_MAX);

        //Debug.Log("Performing attack task " + waitBetweenActions);

        yield return new WaitForSeconds(waitBetweenActions);
    }

    private void CheckPlayerLocation()
    {
        if (KenAnimator.currentPosition.x > transform.position.x)
        {
            if (currentDirection != MoveDirection.Right)
                ChangeDirection(MoveDirection.Right);
        }
        else
        {
            if (currentDirection != MoveDirection.Left)
                ChangeDirection(MoveDirection.Left);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<BoxCollider2D>().tag == playerProjTag)
        {
            // Take damage
            HP -= Projectile.damage;

            // Push back
            if (Projectile.direction == "right")
                rb2d.AddForce(new Vector2(2, 0), ForceMode2D.Impulse);
            else
                rb2d.AddForce(new Vector2(-2, 0), ForceMode2D.Impulse);

        }
    }

    private void ChangeDirection(MoveDirection moveDir)
    {
        if (currentDirection != moveDir)
        {
            switch (moveDir)
            {
                case MoveDirection.Right:
                    transform.Rotate(0, -180, 0);
                    currentDirection = MoveDirection.Right;
                    break;

                case MoveDirection.Left:
                    transform.Rotate(0, 180, 0);
                    currentDirection = MoveDirection.Left;
                    break;
            }
        }
    }
}
