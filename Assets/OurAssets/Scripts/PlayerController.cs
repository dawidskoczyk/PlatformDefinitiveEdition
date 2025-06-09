using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Run, Attack, Die, Jump, Dash}

    [SerializeField] public PlayerState state;
    PlayerState currentState;
    PlayerState previousState;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    [SerializeField] float maxSpeed = 2f;
    [SerializeField] float jumpPower = 6f;
    [SerializeField] float distanceToWall = 0.5f;
    [SerializeField] float groundXSize = 0.6f;

    [SerializeField] LayerMask whatIsGround;

    [SerializeField] int jumpCounter = 0;
    [SerializeField] public bool isLocked = false;
    [SerializeField] bool jump = false;
    [SerializeField] float horizontalInput;
    [SerializeField] bool doubleJumpAfterWall;
    [SerializeField] float boxCastXMiniOffset;
    [SerializeField] public GameObject shoot1;
    [SerializeField] public Transform gunSpot;
    [SerializeField] public float shootSpeed;
    [SerializeField] public float dashForce;
    [SerializeField] public bool canAttack = false;
    [SerializeField] bool dashAttack = false;
    [SerializeField] float dashBrakeTime = 1;
    [SerializeField] float chargeTimer = 0f;
    [SerializeField] public bool slam;
    [SerializeField] public bool upAttack;
    [SerializeField] public bool rightClick;

    bool canWallSlide = true;
    bool wallJump = false;
    bool canChangeState = true;


    void Start()
    {
        state = PlayerState.Idle;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isLocked) // aktualnie znaczy - jesli gracz atakuje
            return;

        TakeInputs();
        
        // umo¿liwia double jump po wyladowwaniu - a moze po prostu sprawdzac ground check i wtedy odnowic ???
        if (state == PlayerState.Idle || state == PlayerState.Run)//(GroundCheck())
        {
            jumpCounter = 0;
        }
    }

    private void FixedUpdate()
    {
        EvaluateState();

        switch (state)
        {
            case PlayerState.Idle:
                BeIdle();
                break;

            case PlayerState.Run:
                Run();
                break;

            case PlayerState.Attack:
                if (!isLocked)
                {
                    //StartCoroutine(GetComponentInChildren<IAttack>().Attack(this));
                }
                break;

            case PlayerState.Dash:
                if (!isLocked)
                {
                    StartCoroutine(Dash());
                }
                break;

            case PlayerState.Die:
                StopAllCoroutines();
                //OnDie();
                break;

            case PlayerState.Jump:
                Jump();
                break;

            default:
                break;
        }
    }

    void TakeInputs()
    {
        if (!canAttack)
            canAttack = Input.GetKeyDown(KeyCode.Mouse0);
        if (!dashAttack)
            dashAttack = Input.GetKeyDown(KeyCode.LeftShift);
        if (!jump) // -> czemu? podczas skoku przecie¿ mo¿na skakaæ ponownie
            jump = Input.GetKeyDown(KeyCode.Space);
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

    void EvaluateState()
    {
        
        if (dashAttack && !isLocked)
        {
            ChangeCharacterState(PlayerState.Dash);
            canAttack = false;
        }
        else if ((canAttack && !isLocked) || (rightClick && !isLocked))
        {
            ChangeCharacterState(PlayerState.Attack);
        }
        else if (jump)
        {;
            ChangeCharacterState(PlayerState.Jump);
        }
        else if (Math.Abs(rb.linearVelocityY) > 1 && !GroundCheck())
        {
            ChangeCharacterState(PlayerState.Jump);
            if(doubleJumpAfterWall== true)
                jumpCounter = 1;
        }
        else if (horizontalInput != 0 && GroundCheck() && rb.linearVelocityY < 0.1f && rb.linearVelocityY > -0.1f)// && state != PlayerState.Jump)
        {
            ChangeCharacterState(PlayerState.Run);
        }
        else if (!Input.anyKey && GroundCheck())
        {
            ChangeCharacterState(PlayerState.Idle);
        }
    }

    
    void BeIdle()
    {
        animator.Play("idle-Animation");
        rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
        rb.linearDamping = 5;
        rb.gravityScale = 1;
        doubleJumpAfterWall = true; // !!!!!!!!! CHECK 1 
    }

    IEnumerator Attack()
    {
        if (dashAttack)
            yield return null;
        //canAttack = false;
        isLocked = true;
        print("attack");
        animator.Play("Shoot_Animation");
        Vector2 savedVelocity = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(0.3f);
       
        //strza³
        Vector2 shotPosition = spriteRenderer.flipX ? transform.TransformPoint(new Vector2(-gunSpot.localPosition.x, gunSpot.localPosition.y)) : gunSpot.position;
        GameObject sh1 = Instantiate(shoot1, shotPosition, Quaternion.identity);
        Vector2 shootDirection = spriteRenderer.flipX ? Vector2.left: Vector2.right;
        sh1.GetComponent<Rigidbody2D>().AddForce(shootDirection * shootSpeed, ForceMode2D.Impulse);
        if(spriteRenderer.flipX) sh1.GetComponent<SpriteRenderer>().flipX = true;

        rb.gravityScale = 1;
        rb.linearVelocity = savedVelocity;
       
        isLocked = false;
        
        if (!GroundCheck()) ChangeCharacterState(PlayerState.Jump);
        else ChangeCharacterState(PlayerState.Idle);

        canAttack = false;
    }

    IEnumerator MeleeAttack()
    {
        isLocked = true;
       
        if (upAttack) {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(0, 1), new Vector2(1, 1), 0);
            Debug.Log("up hit : " + hitCollider);
            upAttack = false;
        }else if (slam)
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
        isLocked = false;
        canAttack = false;
    }

    IEnumerator Dash()
    {
        isLocked = true;
        //Determine dash direction
        Vector2 shootDirection;
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        shootDirection = new Vector2 (xInput, yInput * 2);
        shootDirection.Normalize(); // Ensure consistent dash speed

        if (shootDirection == Vector2.zero)
        {
            if (spriteRenderer.flipX)
                shootDirection = Vector2.left;
            else
                shootDirection = Vector2.right;
        }

        print("add force - dash");
        rb.AddForce(shootDirection * dashForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f);

        //dashAttack = false;
        Invoke(nameof(DashEnd), dashBrakeTime);
        isLocked = false;

        ChangeCharacterState(PlayerState.Idle);
        if (!GroundCheck()) ChangeCharacterState(PlayerState.Jump);
    }

    void Run()
    {  
        rb.linearDamping = 5;
        animator.Play("Run_Animation");
        doubleJumpAfterWall = true; // <- idk
        if (Input.GetKey(KeyCode.A) && GroundCheck())
        {
            rb.AddForce(new Vector2(-maxSpeed, 0), ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.D) && GroundCheck())
        {
            rb.AddForce(new Vector2(maxSpeed, 0), ForceMode2D.Force);
        }

        Vector2 currentVelocity = rb.linearVelocity;
        Vector2 clampedVelocity = currentVelocity;
        clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed, maxSpeed);
        Vector2 lerpedVelocity = Vector2.Lerp(currentVelocity, clampedVelocity, 10);
        rb.linearVelocity = lerpedVelocity;
    }

    void Jump()
    {
        //dodac maxjumps - max liczba mozliwych skokow
        // SPADANIE
        if (rb.linearVelocityY < -1)
        {
            animator.Play("fall");
            rb.linearDamping = 3;
        }

        rb.linearDamping = 2; // <- podczas skoku a jaki jest normalnie?

        // pierwszy skok (z ziemi)
        if (jump && GroundCheck())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCounter = 1;
            animator.Play("jump");

            canWallSlide = false;
            Invoke(nameof(UnlockWallSlide), 0.4f);  // <- zapobiego przyklejaniu sie do sciany podaczas skoku przy niej (przykleja sie dopiero po 0.4s)
        }
        else if (jump && jumpCounter == 1 && !(WallCheckLeft() || WallCheckRight()))
        {
            if(!doubleJumpAfterWall)  return;
            animator.Play("FrontFlip");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCounter = 2; // Oznacza, ¿e wykorzystaliœmy oba skoki
            doubleJumpAfterWall = false;
            // /\ wcale nie, oznacza rest liczby mo¿liwych skokow (chyba)

        }
        else if (jump && (WallCheckLeft() || WallCheckRight()))
        {
            animator.Play("jump");
            wallJump = true;
            //if(jumpCounter == 1) jumpCounter = 0; //po dotkniêciu œciany ka¿dy skok to double jump
            rb.gravityScale = 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹

            if(horizontalInput == 0)
            {
                if (spriteRenderer.flipX)
                    horizontalInput = 1;
                else
                    horizontalInput = -1;
            }

            if((horizontalInput == -1 && WallCheckLeft()) ||(horizontalInput == 1 && WallCheckRight()))
                rb.AddForce(new Vector2(horizontalInput * -30f, jumpPower), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(horizontalInput * 30f, jumpPower), ForceMode2D.Impulse);

            jumpCounter = 1; // Oznacza, ¿e wykorzystaliœmy oba skoki
            Invoke(nameof(ChangeJumpReady), 0.2f);

            canWallSlide = false;
            Invoke(nameof(UnlockWallSlide), 0.2f);
            
        }

        else if (((WallCheckLeft() && horizontalInput == -1) || (WallCheckRight() && horizontalInput == 1)) && canWallSlide)// && !wallJump && Math.Abs(rb.linearVelocityY) < 1)
        {
            //if (jumpCounter == 1) jumpCounter = 0;
            animator.Play("WallSlide");
            rb.gravityScale = 0.3f;
            rb.linearVelocity = new Vector2(0, 0);
        }
        else
        {
            rb.gravityScale = 1;
            rb.AddForce(new Vector2(Input.GetAxis("Horizontal") * maxSpeed, 0), ForceMode2D.Force);
            Vector2 currentVelocity = rb.linearVelocity;
            Vector2 clampedVelocity = currentVelocity;
            if (slam)
            {
                rb.AddForce(new Vector2(0, -jumpPower), ForceMode2D.Impulse);
                slam = false;
                Collider2D hitCollider = Physics2D.OverlapBox(transform.position - new Vector3(0, 1f), new Vector2(2, -1), 0f);
                animator.Play("slam");
            }
        }

        jump = false;
       
    }

    public void ChangeCharacterState(PlayerState playerState)
    {
        state = playerState;
        //isLocked = true;
    }

    void ChangeJumpReady()
    {
        wallJump = false;
    }

    public bool GroundCheck()
    {
        return Physics2D.BoxCast(transform.position - new Vector3(-GetComponent<BoxCollider2D>().offset.x + boxCastXMiniOffset,0.4f,0), new Vector2(groundXSize, 0.3f),0f, Vector2.down, 0.4f, whatIsGround);
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
    

    void UnlockStateChange()
    {
        canChangeState = true;
    }

    void Unlocked()
    {
        isLocked = false;
    }

    void UnlockWallSlide()
    {
        canWallSlide = true;
    }

    void JumpToFalse()
    {
        jump = false;
    }

    void DashEnd()
    {
        dashAttack = false;
    }


    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.right * distanceToWall, Color.cyan);
        Debug.DrawRay(transform.position, -transform.right * distanceToWall, Color.cyan);

        Gizmos.DrawWireCube(transform.position - new Vector3(0,1f) , new Vector2(2, 1));
    }

}


