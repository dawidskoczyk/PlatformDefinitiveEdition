using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Run, Attack, Die, Jump, DashAttack }

    [SerializeField] PlayerState state;
    PlayerState currentState;
    PlayerState previousState;
    Animator animator;
    Rigidbody2D rb;

    [SerializeField] bool isLocked = false;
    [SerializeField] float maxSpeed = 2f;
    [SerializeField] float jumpPower = 6f;
    [SerializeField] float moveSpeed = 4f;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite jumpSprite;
    [SerializeField] Sprite fallSprite;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float smoothTime = 0.2f;
    [SerializeField] float distanceToWall = 0.5f;
    [SerializeField] float groundXSize = 0.6f;

    [SerializeField]int jumpCounter = 0;
    bool wallJump = false;
    bool canChangeState = true;
    [SerializeField] bool jump = false;
    [SerializeField] float horizontalInput;
    [SerializeField] bool groudnededed;
    [SerializeField] bool doubleJumpAfterWall;
    [SerializeField] float boxCastXMiniOffset;
    [SerializeField] GameObject shoot1;
    [SerializeField] Transform gunSpot;
    [SerializeField] float shootSpeed;
    [SerializeField] bool canAttack = false;
    [SerializeField] bool dashAttack = false;
    [SerializeField] float chargeTimer = 0f;
    [SerializeField] bool slam;
    [SerializeField] bool upAttack;

    void Start()
    {
        state = PlayerState.Idle;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isLocked) // aktualnie znaczy - jesli gracz atakuje
            return;

        TakeInputs();
        EvaluateState();

        if (state == PlayerState.Idle)//(GroundCheck())
        {
            //print("ground");
            //jump = false;
            jumpCounter = 0;
        }

    }

    private void FixedUpdate()
    {
        groudnededed = GroundCheck();
        switch (state)
        {
            case PlayerState.Idle:
                animator.Play("idle-Animation");
                rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
                rb.linearDamping = 5;
                rb.gravityScale = 1;
                doubleJumpAfterWall = true;
                //StopAllCoroutines();
                break;
            case PlayerState.Run:
                Run();
                break;
            case PlayerState.Attack:
                if (!isLocked)
                    StartCoroutine(MeleeAttack());
                //StartCoroutine(Attack());
                break;
            case PlayerState.DashAttack:
                if (!isLocked)
                    StartCoroutine(DashAttack());
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

    IEnumerator DashAttack()
    {
        isLocked = true;
        rb.gravityScale = 0.1f;
        // Charging duration (adjust as needed)
        float chargeDuration = 1f;

        // Charge visual or gameplay feedback can be added here
        while (chargeTimer < chargeDuration)
        {
            // Optional: Add charging mechanics
            // For example, visual charge effect, building up power, etc.
            chargeTimer += Time.deltaTime;
            if (Input.GetMouseButtonUp(0)) // Left mouse button
            {
                break;
            }
            // Allow cancellation during charge
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                // Reset state if canceled
                isLocked = false;
                dashAttack = false;
                ChangeCharacterState(PlayerState.Idle);
                yield break;
            }
            yield return null;
        }

        // Determine dash direction
        Vector3 shootDirection;
        shootDirection = Input.mousePosition;
        shootDirection.z = 0.0f;
        shootDirection = Camera.main.ScreenToWorldPoint(shootDirection);
        shootDirection = shootDirection - transform.position;
        shootDirection.Normalize(); // Ensure consistent dash speed

        // Perform dash with potentially increased force based on charge time
        float dashForce = 500f * (1f + (chargeTimer / chargeDuration));
        rb.AddForce(shootDirection * dashForce, ForceMode2D.Impulse);

        // Dash duration
        yield return new WaitForSeconds(0.3f);
        rb.gravityScale = 1f;
        // Reset states
        isLocked = false;
        dashAttack = false;
        ChangeCharacterState(PlayerState.Idle);
        if (!GroundCheck()) ChangeCharacterState(PlayerState.Jump);
        chargeTimer = 0f;
    }

    void Run()
    {
        rb.linearDamping = 5;
        animator.Play("Run_Animation");
        if (Input.GetKey(KeyCode.A) && GroundCheck())
        {
            rb.AddForce(new Vector2(-moveSpeed, 0), ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.D) && GroundCheck())
        {
            rb.AddForce(new Vector2(moveSpeed, 0), ForceMode2D.Force);
        }

        Vector2 currentVelocity = rb.linearVelocity;
        Vector2 clampedVelocity = currentVelocity;
        clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed, maxSpeed);
        Vector2 lerpedVelocity = Vector2.Lerp(currentVelocity, clampedVelocity, 10);
        rb.linearVelocity = lerpedVelocity;
    }
    void Jump()
    {
        //jeszcze do przerobienia case kiedy postaæ biegnie, spada z platformy i dotyka œciany, bo wtedy siê jej nie ³apie bo ³apanie œciany jest w stanie skoku. \
        //Bêdzie trzeba ca³¹ obs³ugê œcian jakoœ przenieœæ albo obs³u¿yæ ten wyj¹tek kopiuj¹c fragment kodu do stanu run

        //dodac maxjumps - max liczba mozliwych skokow

        if (rb.linearVelocityY < -1)
        {
            animator.Play("fall");
            rb.linearDamping = 3;
          
        }   
        else
            animator.Play("jump");

        rb.linearDamping = 2;
        if (jump && GroundCheck())// && jumpCounter == 0)
        {
            print("jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCounter = 1;
        }
        else if (jump && jumpCounter == 1 && !(WallCheckLeft() || WallCheckRight()))
        {
            if(!doubleJumpAfterWall)  return; 
            print("double jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCounter = 0; // Oznacza, ¿e wykorzystaliœmy oba skoki
            doubleJumpAfterWall = false;
            // /\ wcale nie, oznacza rest liczby mo¿liwych skokow (chyba)

        }
        else if (jump && horizontalInput != 0 && (WallCheckLeft() || WallCheckRight()))
        {
            print("wall jump"); // i tu te¿ trzeba pomyœleæ nad czasem kojota,
            wallJump = true;
            //if(jumpCounter == 1) jumpCounter = 0; //po dotkniêciu œciany ka¿dy skok to double jump
            rb.gravityScale = 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹

            if((horizontalInput == -1 && WallCheckLeft()) ||(horizontalInput == 1 && WallCheckRight()))
                rb.AddForce(new Vector2(horizontalInput * -30f, jumpPower), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(horizontalInput * 30f, jumpPower), ForceMode2D.Impulse);

            jumpCounter = 1; // Oznacza, ¿e wykorzystaliœmy oba skoki
            Invoke(nameof(ChangeJumpReady), 0.2f);
            
        }

        if (((WallCheckLeft() && horizontalInput == -1) || (WallCheckRight() && horizontalInput == 1)) && !wallJump)
        {
            print("wall slide");
            //if (jumpCounter == 1) jumpCounter = 0;
            rb.gravityScale = 0.3f;
            rb.linearVelocity = new Vector2(0, 0);
        }
        else
        {
            print("jump else");
            rb.gravityScale = 1;
            rb.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed, 0), ForceMode2D.Force);
            Vector2 currentVelocity = rb.linearVelocity;
            Vector2 clampedVelocity = currentVelocity;
            if (slam)
            {
                rb.AddForce(new Vector2(0, -jumpPower), ForceMode2D.Impulse);
                slam = false;
                Collider2D hitCollider = Physics2D.OverlapBox(transform.position - new Vector3(0, 1f), new Vector2(2, -1), 0f);
                Debug.Log("SLam : " + hitCollider);
            }

            //// U¿yj bardziej rozs¹dnej wartoœci mno¿nika, np. 0.5f dla powietrza
            //clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed * 0.5f, maxSpeed * 0.5f);

            //// U¿yj wartoœci smoothTime, która zapewni zauwa¿aln¹ zmianê
            //Vector2 lerpedVelocity = Vector2.Lerp(currentVelocity, clampedVelocity, 10); //smoothTime + Time.deltaTime);
            //rb.linearVelocity = lerpedVelocity;

            //// Debug - wyœwietlanie wartoœci w konsoli, aby zobaczyæ, czy ograniczenie dzia³a
            //Debug.Log($"Current: {currentVelocity.x}, Clamped: {clampedVelocity.x}, Lerped: {lerpedVelocity.x}");
        }

        //if (GroundCheck())
        //    state = PlayerState.Idle;
        
        jump = false;
       
    }

    void EvaluateState()
    {
        if (!Input.anyKey && GroundCheck())
        {
            ChangeCharacterState(PlayerState.Idle);
        }
        else if (dashAttack && !isLocked)
        {
            ChangeCharacterState(PlayerState.DashAttack);
            canAttack = false;
        }
        else if (canAttack && !isLocked )
        {
            ChangeCharacterState(PlayerState.Attack);
        }
        else if (jump)
        {
            ChangeCharacterState(PlayerState.Jump);
        }
        else if (horizontalInput != 0 && GroundCheck() && rb.linearVelocityY < 0.1f && rb.linearVelocityY > -0.1f)// && state != PlayerState.Jump)
        {
            ChangeCharacterState(PlayerState.Run);
        }
    }

    void TakeInputs()
    {
        if(!canAttack)
            canAttack = Input.GetKeyDown(KeyCode.Mouse0);

        if (!dashAttack)
            dashAttack = Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse1);

        horizontalInput = Input.GetAxisRaw("Horizontal");
        if(horizontalInput < 0) {spriteRenderer.flipX = true; }
        else if (horizontalInput > 0) { spriteRenderer.flipX = false; }
        if (!jump)
            jump = Input.GetKeyDown(KeyCode.Space);
        if (!slam)
            slam = Input.GetKeyDown(KeyCode.S);
        if (!upAttack)
            upAttack = Input.GetKeyDown(KeyCode.W);
    }

    void ChangeCharacterState(PlayerState playerState)
    {
        state = playerState;
        //isLocked = true;
    }

    void ChangeJumpReady()
    {
        wallJump = false;
    }

    bool GroundCheck()
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

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.right * distanceToWall, Color.cyan);
        Debug.DrawRay(transform.position, -transform.right * distanceToWall, Color.cyan);

        Gizmos.DrawWireCube(transform.position - new Vector3(0,1f) , new Vector2(2, 1));
    }

}


