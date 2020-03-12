using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KenAnimator : MonoBehaviour
{
    const int STATE_IDLE = 0;
    const int STATE_WALK = 1;
    const int STATE_CROUCH = 2;
    const int STATE_JUMP = 3;
    const int STATE_ATTACK = 4;
    private int currentState;

    const int WALK_SPEED = 8;
    const int MOVEMENT_FORCE = 6;
    const int MAX_MOVE_VELOCITY = -5;
    const int JUMP_FORCE = 80;

    private int dynamicMoveForce = MOVEMENT_FORCE;
    private bool isInAir = false;
    private float fireRate = 0.917f;
    private float nextFire = 0.0f;

    private int HP = 100;

    private MoveDirection currentDirection = MoveDirection.Left;
    private Rigidbody2D rb;
    public Rigidbody2D projectileRight;
    public Rigidbody2D projectileLeft;
    private Animator animator = new Animator();

    private BoxCollider2D boxCollider;
    private float boxColCrouchHeight;
    private float boxColNormalHeight;
    private float boxColWidth;

    public static Vector3 currentPosition;

    enum MoveDirection
    {
        Left,
        Right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        boxCollider = GetComponent<BoxCollider2D>();
        boxColNormalHeight = boxCollider.size.y;
        boxColWidth = boxCollider.size.x;
        boxColCrouchHeight = boxColNormalHeight * 0.65f;
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition = transform.position;

        if (animator.GetInteger("state") != 0)
            animator.SetInteger("state", currentState = STATE_IDLE);

        if (Input.GetKey(KeyCode.A))
        {
            ChangeDirection(MoveDirection.Left);

            if (rb.velocity.x > MAX_MOVE_VELOCITY)
                rb.AddForce(Vector2.left * dynamicMoveForce, ForceMode2D.Impulse);

            animator.SetInteger("state", currentState = STATE_WALK);
        }

        if (Input.GetKey(KeyCode.D))
        {
            ChangeDirection(MoveDirection.Right);

            if (rb.velocity.x < Mathf.Abs(MAX_MOVE_VELOCITY))
                rb.AddForce(Vector2.right * dynamicMoveForce, ForceMode2D.Impulse);

            animator.SetInteger("state", currentState = STATE_WALK);
        }

        if (Input.GetKeyDown(KeyCode.W))
            if (!isInAir)
            {
                rb.AddForce(new Vector2(0, JUMP_FORCE), ForceMode2D.Impulse);
                animator.SetInteger("state", currentState = STATE_JUMP);
            }

        if (Input.GetKey(KeyCode.S))
        {
            animator.SetInteger("state", currentState = STATE_CROUCH);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetInteger("state", currentState = STATE_ATTACK);

            if (currentDirection == MoveDirection.Right)
            {
                if (Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    Instantiate(projectileRight, transform.position + new Vector3(3, 1), Quaternion.identity).transform.localScale = new Vector3(0.5f, 0.5f, 0);
                }
            }
            else
            {
                if (Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    Instantiate(projectileLeft, transform.position + new Vector3(-3, 1), Quaternion.identity).transform.localScale = new Vector3(0.5f, 0.5f, 0);

                }
            }
        }

        CheckIfAttackOrCrouch();
        // SetBoxColliderHeight();
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

    private void CheckIfAttackOrCrouch()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch_simple"))
            dynamicMoveForce = 0;
        else
            dynamicMoveForce = MOVEMENT_FORCE;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch_simple"))
            boxCollider.size = new Vector2(boxColWidth, boxColCrouchHeight);
        else
            boxCollider.size = new Vector2(boxColWidth, boxColNormalHeight);
    }

    private void SetBoxColliderHeight()
    {
        if (currentState == STATE_CROUCH)
            boxCollider.size = new Vector2(boxColWidth, boxColCrouchHeight);
        else
            boxCollider.size = new Vector2(boxColWidth, boxColNormalHeight);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isInAir = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isInAir = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isInAir = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyProjectile")
        {
            HP -= (int)EnemyProjectile.damage;
            Debug.Log("HP: " + HP);

            // Change health bar display
            GameObject HPBar = GameObject.Find("Bar");
            HPBar.transform.localScale = new Vector3((float)HP / 100.0f, 1.0f);
        }
    }
}

//transform.Translate(-Vector3.right * walkSpeed * Time.deltaTime);
//transform.Translate(Vector3.left * walkSpeed * Time.deltaTime);
