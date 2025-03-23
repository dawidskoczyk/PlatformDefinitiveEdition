using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Run, Attack, Die, Jump }

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
    [SerializeField]bool jump = false;
    [SerializeField]float horizontalInput;
    [SerializeField] bool groudnededed;
    [SerializeField] float boxCastXMiniOffset;



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
        
        TakeInputs();

        if (isLocked) // aktualnie znaczy - jesli gracz atakuje
            return;

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
                //StopAllCoroutines();
                break;
            case PlayerState.Run:
                Run();
                break;
            case PlayerState.Attack:
                StartCoroutine(Attack());
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
        isLocked = true;

        animator.Play("Shoot_Animation");
        rb.gravityScale = 0;
        Vector2 savedVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector2 (0, 0);
        yield return new WaitForSeconds(0.3f);

        isLocked = false;
        rb.gravityScale = 1f;
        rb.linearVelocity = savedVelocity;
        if (!GroundCheck()) ChangeCharacterState(PlayerState.Jump);
        else ChangeCharacterState(PlayerState.Idle);
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
            animator.Play("fall");
        else
            animator.Play("jump");

        rb.linearDamping = 5;
        if (jump && GroundCheck())// && jumpCounter == 0)
        {
            print("jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCounter = 1;
        }
        else if (jump && jumpCounter == 1 && !(WallCheckLeft() || WallCheckRight()))
        {
            print("double jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCounter = 0; // Oznacza, ¿e wykorzystaliœmy oba skoki
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

            jumpCounter = 0; // Oznacza, ¿e wykorzystaliœmy oba skoki
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
        else if (Input.GetKeyDown(KeyCode.Mouse0))
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
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (!jump)
            jump = Input.GetKeyDown(KeyCode.Space);
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
    }

}


