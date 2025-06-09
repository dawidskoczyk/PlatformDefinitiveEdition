using UnityEngine;
using System;
using System.Collections;

public class PlayerControllerSM : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private Animator animator;

    private StateMachine stateMachine;

    // Input
    private float horizontalInput;
    private bool jumpPressed;
    private bool dashPressed;
    private bool attackPressed;
    // ########################################################3


    [HideInInspector] public SpriteRenderer spriteRenderer;

    [SerializeField] float maxSpeed = 1f;
    [SerializeField] float distanceToWall = 0.5f;
    [SerializeField] float groundXSize = 0.6f;

    [SerializeField] LayerMask whatIsGround;

    //[SerializeField] public bool isLocked = false;
    //[SerializeField] bool doubleJumpAfterWall;
    [SerializeField] float boxCastXMiniOffset;
    //[SerializeField] public GameObject shoot1;
    [SerializeField] public Transform gunSpot;
    //[SerializeField] public float shootSpeed;
    //[SerializeField] public float dashForce;
    [SerializeField] public bool leftClick = false;
    //[SerializeField] bool dashAttack = false;
    //[SerializeField] float dashBrakeTime = 1;
    //[SerializeField] float chargeTimer = 0f;
    [SerializeField] public bool isAttacking;
    [SerializeField] public bool slam;
    [SerializeField] public bool upAttack;
    [SerializeField] public bool rightClick;

    [SerializeField] public int jumpCounter;

    [SerializeField] int maxJumpNumber;

    bool canWallSlide = true;
    bool wallJump = false;
    //bool canChangeState = true;

    // ########################################################3

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Inicjalizacja maszyny stanów z referencj¹ do kontrolera
        stateMachine = new StateMachine(this);

        stateMachine.Initialize(stateMachine.idleState);
    }

    private void Update()
    {
        // Zbierz input
        TakeInput();

        // Update logiki
        stateMachine.Update();

        print(stateMachine.CurrentState);
    }

    void TakeInput()
    {
        if (!leftClick)
            leftClick = Input.GetKeyDown(KeyCode.Mouse0);
        if (!rightClick)
            rightClick = Input.GetKeyDown(KeyCode.Mouse1);
        //if (!dashAttack)
        //    dashAttack = Input.GetKeyDown(KeyCode.LeftShift);
        //if (!jump) // -> czemu? podczas skoku przecie¿ mo¿na skakaæ ponownie
        if (!jumpPressed)
            jumpPressed = Input.GetKeyDown(KeyCode.Space);
        if (!slam)
            slam = Input.GetKeyDown(KeyCode.S);
        if (!upAttack)
            upAttack = Input.GetKeyDown(KeyCode.W);

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput < 0)
            spriteRenderer.flipX = true;
        else if (horizontalInput > 0)
            spriteRenderer.flipX = false;
    }

    public void Move(float velocityX)
    {
        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocityY);

        //doubleJumpAfterWall = true; // <- idk
        //if (Input.GetKey(KeyCode.A) && GroundCheck())
        //{
        //    rb.AddForce(new Vector2(-maxSpeed, 0), ForceMode2D.Force);
        //}
        //if (Input.GetKey(KeyCode.D) && GroundCheck())
        //{
        //    rb.AddForce(new Vector2(maxSpeed, 0), ForceMode2D.Force);
        //}

        //Vector2 currentVelocity = rb.linearVelocity;
        //Vector2 clampedVelocity = currentVelocity;
        //clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed, maxSpeed);
        //Vector2 lerpedVelocity = Vector2.Lerp(currentVelocity, clampedVelocity, 10);
        //rb.linearVelocity = lerpedVelocity;

    }

    public void Jump()
    {
        if (stateMachine.CurrentState != stateMachine.jumpState)
            return;

        if (rb.linearVelocityY < -1)
        {
            animator.Play("fall");
            rb.linearDamping = 3;
        }

        rb.linearDamping = 2;

        if (jumpPressed && jumpCounter == 0 && IsGrounded()) // pierwszy skok
        {
            animator.Play("jump");

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            
            jumpCounter = 1;

            canWallSlide = false;
            Invoke(nameof(UnlockWallSlide), 0.4f);  // <- naprawia blok skoku przy scianie
        }
        // ########################################

        else if (jumpPressed && jumpCounter > 0 && jumpCounter < maxJumpNumber && !(WallCheckLeft() || WallCheckRight())) // kolejne skoki w powietrzu
        {
            //if (!doubleJumpAfterWall) return;
            animator.Play("FrontFlip");

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

            jumpCounter++;
            //doubleJumpAfterWall = false;
        }
        else if (jumpPressed && (WallCheckLeft() || WallCheckRight())) // skoki na scianach
        {
            animator.Play("jump");

            rb.gravityScale = 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹

            if (horizontalInput == 0)
            {
                if (spriteRenderer.flipX)
                    horizontalInput = 1;
                else
                    horizontalInput = -1;
            }

            if ((horizontalInput == -1 && WallCheckLeft()) || (horizontalInput == 1 && WallCheckRight()))
                rb.AddForce(new Vector2(horizontalInput * -30f, jumpForce), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(horizontalInput * 30f, jumpForce), ForceMode2D.Impulse);

            //jumpCounter = 1; // Oznacza, ¿e wykorzystaliœmy oba skoki
            Invoke(nameof(ChangeWallJumpFalse), 0.2f);

            canWallSlide = false;
            Invoke(nameof(UnlockWallSlide), 0.2f);

        }

        else if (((WallCheckLeft() && horizontalInput == -1) || (WallCheckRight() && horizontalInput == 1)) && canWallSlide)// && !wallJump && Math.Abs(rb.linearVelocityY) < 1)
        {
            animator.Play("WallSlide");
            rb.gravityScale = 0.3f;
            rb.linearVelocity = new Vector2(0, 0);
        }
        else
        {
            rb.gravityScale = 1;
            rb.AddForce(new Vector2(Input.GetAxis("Horizontal") * 100, 0), ForceMode2D.Force);
            Vector2 currentVelocity = rb.linearVelocity;
            Vector2 clampedVelocity = currentVelocity;
            clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed, maxSpeed);
            rb.linearVelocityX = clampedVelocity.x;
            if (slam)
            {
                rb.AddForce(new Vector2(0, -jumpForce), ForceMode2D.Impulse);
                slam = false;
                Collider2D hitCollider = Physics2D.OverlapBox(transform.position - new Vector3(0, 1f), new Vector2(2, -1), 0f);
                animator.Play("slam");
            }
        }
        jumpPressed = false;
    }

    //void Dash()
    //{
    //    isLocked = true;
    //    //Determine dash direction
    //    Vector2 shootDirection;
    //    float xInput = Input.GetAxisRaw("Horizontal");
    //    float yInput = Input.GetAxisRaw("Vertical");
    //    shootDirection = new Vector2(xInput, yInput * 2);
    //    shootDirection.Normalize(); // Ensure consistent dash speed

    //    if (shootDirection == Vector2.zero)
    //    {
    //        if (spriteRenderer.flipX)
    //            shootDirection = Vector2.left;
    //        else
    //            shootDirection = Vector2.right;
    //    }

    //    print("add force - dash");
    //    rb.AddForce(shootDirection * dashForce, ForceMode2D.Impulse);

    //    //yield return new WaitForSeconds(0.1f);

    //    //dashAttack = false;
    //    Invoke(nameof(DashEnd), dashBrakeTime);
    //    isLocked = false;

    //    ChangeCharacterState(PlayerState.Idle);
    //    if (!GroundCheck()) ChangeCharacterState(PlayerState.Jump);
    //}

    public bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position - new Vector3(-GetComponent<BoxCollider2D>().offset.x + boxCastXMiniOffset, 0.4f, 0), new Vector2(groundXSize, 0.3f), 0f, Vector2.down, 0.4f, whatIsGround);
    }

    public void SetAnimationTrigger(string trigger)
    {
        if (animator != null)
            animator.SetTrigger(trigger);
    }

    public bool IsAttacking()
    {
        // Opcjonalne: sprawdŸ stan animacji
        return false;
    }

    IEnumerator MeleeAttack()
    {
        //isLocked = true;

        if (upAttack)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(0, 1), new Vector2(1, 1), 0);
            Debug.Log("up hit : " + hitCollider);
            upAttack = false;
        }
        else if (slam)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(0, -1), new Vector2(1, 1), 0);
            Debug.Log("down hit : " + hitCollider);
            slam = false;
        }
        else if (spriteRenderer.flipX)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(-1, 0), new Vector2(1, 1), 0);
            Debug.Log("left hit : " + hitCollider);

        }
        else
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(1, 0), new Vector2(1, 1), 0);
            Debug.Log("right hit : " + hitCollider);
        }

        yield return new WaitForSeconds(0.3f);
        //isLocked = false;
        //canAttack = false;
    }

    void UnlockWallSlide()
    {
        canWallSlide = true;
    }

    void ChangeWallJumpFalse()
    {
        wallJump = false;
    }

    bool WallCheckRight()
    {
        return Physics2D.Raycast(transform.position, transform.right, distanceToWall, whatIsGround);
        //return Physics2D.BoxCast(transform.position - new Vector3(0, 0.1f, 0), new Vector2(0.3f, 0.8f), 0f, Vector2.right, 0.4f, whatIsGround);
    }

    bool WallCheckLeft()
    {
        return Physics2D.Raycast(transform.position, -transform.right, distanceToWall, whatIsGround);
        //return Physics2D.BoxCast(transform.position - new Vector3(0, 0.1f, 0), new Vector2(0.3f, 0.8f), 0f, Vector2.left, 0.4f, whatIsGround);
    }


    public float GetHorizontalInput() => horizontalInput;
    public bool IsJumpPressed() => jumpPressed;
    public bool IsDashPressed() => dashPressed;
    public bool IsAttackPressed() => attackPressed;

    public Rigidbody2D GetRigidbody() => rb;
    public Animator GetAnimator() => animator;
    public StateMachine GetStateMachine() => stateMachine;
}
