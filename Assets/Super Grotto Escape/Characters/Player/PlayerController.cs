using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    int jumpCounter = 0;
    bool wallJump = false;
    bool canChangeState = true;
    bool jump = false;
    float horizontalInput;
    //bool jumpClicked = false;
    public enum PlayerState { Idle, Run, Attack, Die, Jump }


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
        if (isLocked)
            return;
        EvaluateState();
    }

    private void FixedUpdate()
    {



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
        //Invoke(nameof(Unlocked), 0.2f);
        

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

        rb.linearDamping = 2;
        if (jump && GroundCheck() && jumpCounter == 0)
        {
            print("skok");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCounter = 1;
        }
        else if (jump && jumpCounter == 1)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCounter = 0; // Oznacza, ¿e wykorzystaliœmy oba skoki
           
        }
        else if (jump && (WallCheckLeft() || WallCheckRight()))
        {
            wallJump = true;
            rb.gravityScale = 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(Input.GetAxis("Horizontal")*3f, jumpPower), ForceMode2D.Impulse);
            jumpCounter = 0; // Oznacza, ¿e wykorzystaliœmy oba skoki
            Invoke(nameof(ChangeJumpReady),0.2f);
            
        }

            if ((WallCheckLeft() || WallCheckRight()) && (horizontalInput != 0) && !wallJump)
            {
                rb.gravityScale = 0.3f;
                rb.linearVelocity = new Vector2(0, 0);
            }
            else
            {
                rb.gravityScale = 1;
                rb.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed, 0), ForceMode2D.Force);
                Vector2 currentVelocity = rb.linearVelocity;
                Vector2 clampedVelocity = currentVelocity;

                // U¿yj bardziej rozs¹dnej wartoœci mno¿nika, np. 0.5f dla powietrza
                clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed * 0.5f, maxSpeed * 0.5f);

                // U¿yj wartoœci smoothTime, która zapewni zauwa¿aln¹ zmianê
                Vector2 lerpedVelocity = Vector2.Lerp(currentVelocity, clampedVelocity, 10); //smoothTime + Time.deltaTime);
                rb.linearVelocity = lerpedVelocity;

                // Debug - wyœwietlanie wartoœci w konsoli, aby zobaczyæ, czy ograniczenie dzia³a
                Debug.Log($"Current: {currentVelocity.x}, Clamped: {clampedVelocity.x}, Lerped: {lerpedVelocity.x}");
            }
        
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
        else if (jump )
        {
            ChangeCharacterState(PlayerState.Jump );
        }
        else if (horizontalInput != 0 && GroundCheck() && rb.linearVelocityY < 0.1f && rb.linearVelocityY > -0.1f && state != PlayerState.Jump)
        {
            ChangeCharacterState(PlayerState.Run);
        }
    }

    void TakeInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");

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
        return Physics2D.BoxCast(transform.position - new Vector3(0,0.4f,0), new Vector2(0.8f, 0.3f),0f, Vector2.down, 0.4f, whatIsGround);
    }

    bool WallCheckRight()
    {
        return Physics2D.BoxCast(transform.position - new Vector3(0, 0.1f, 0), new Vector2(0.3f, 0.8f), 0f, Vector2.right, 0.4f, whatIsGround);
    }

    bool WallCheckLeft()
    {
        return Physics2D.BoxCast(transform.position - new Vector3(0, 0.1f, 0), new Vector2(0.3f, 0.8f), 0f, Vector2.left, 0.4f, whatIsGround);
    }
        void OnDrawGizmos()
    {
        // Pozycja pocz¹tkowa boxa
        Vector3 origin = transform.position - new Vector3(0, 0.4f, 0);

        //// Rysowanie BoxCast
        //DrawBoxCastGizmo(origin, 0.8f, 0.2f, 0.1f);

        // Pozycja pocz¹tkowa BoxCast
        Vector3 origin1 = transform.position - new Vector3(0, 0.1f,0);

        // Wymiary BoxCast
        float width = 0.3f;
        float height = 0.8f;

        // Kierunek i odleg³oœæ rzutu
        Vector3 direction = Vector3.right;
        float distance = 0.4f;

        // Rysowanie górnego raya
        Vector3 topRayStart = origin1 + new Vector3(0, height / 2, 0);
        Vector3 topRayEnd = topRayStart + direction * distance;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(topRayStart, topRayEnd);

        // Rysowanie dolnego raya
        Vector3 bottomRayStart = origin1 - new Vector3(0, height / 2, 0);
        Vector3 bottomRayEnd = bottomRayStart + direction * distance;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomRayStart, bottomRayEnd);

        // Opcjonalnie - linie ³¹cz¹ce koñce rayów dla lepszej wizualizacji
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(topRayStart, bottomRayStart);
        Gizmos.DrawLine(topRayEnd, bottomRayEnd);

        // Pozycja pocz¹tkowa BoxCast (z przesuniêciem o 0.1f w dó³)
        Vector3 leftOrigin = transform.position - new Vector3(0, 0.1f, 0);

        // Wymiary BoxCast
        float leftWidth = 0.3f;
        float leftHeight = 0.8f;

        // Kierunek i odleg³oœæ rzutu
        Vector3 leftDirection = Vector2.left;
        float leftDistance = 0.4f;

        // Rysowanie górnego raya
        Vector3 leftTopRayStart = leftOrigin + new Vector3(0, leftHeight / 2, 0);
        Vector3 leftTopRayEnd = leftTopRayStart + leftDirection * leftDistance;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftTopRayStart, leftTopRayEnd);

        // Rysowanie dolnego raya
        Vector3 leftBottomRayStart = leftOrigin - new Vector3(0, leftHeight / 2, 0);
        Vector3 leftBottomRayEnd = leftBottomRayStart + leftDirection * leftDistance;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftBottomRayStart, leftBottomRayEnd);

        // Opcjonalnie - linie ³¹cz¹ce koñce rayów dla lepszej wizualizacji
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(leftTopRayStart, leftBottomRayStart);
        Gizmos.DrawLine(leftTopRayEnd, leftBottomRayEnd);
    }

    private void DrawRayWithHitCheck(Vector2 start, Vector2 direction, LayerMask layer, Color noHitColor, Color hitColor)
    {
        RaycastHit2D hit = Physics2D.Raycast(start, direction.normalized, direction.magnitude, layer);

        if (hit.collider != null)
        {
            // Jeœli jest kolizja, narysuj czerwony promieñ do punktu kolizji
            Gizmos.color = hitColor;
            Gizmos.DrawLine(start, hit.point);

            // Narysuj ma³¹ sferê w miejscu kolizji
            Gizmos.DrawSphere(hit.point, 0.05f);
        }
        else
        {
            // Jeœli nie ma kolizji, narysuj zielony promieñ na pe³n¹ d³ugoœæ
            Gizmos.color = noHitColor;
            Gizmos.DrawLine(start, start + direction);
        }
    }

    void UnlockStateChange()
    {
        canChangeState = true;
    }

    void Unlocked()
    {
        isLocked = false;
    }

    private void DrawBoxCastGizmo(Vector3 origin, float width, float height, float distance)
    {
        // Narysuj box w pozycji pocz¹tkowej
        Gizmos.color = Color.green;
        Vector3 boxSize = new Vector3(width, height, 0.01f);
        Gizmos.DrawWireCube(origin, boxSize);

        // Narysuj box w pozycji koñcowej
        Gizmos.color = Color.red;
        Vector3 endPosition = origin + Vector3.down * distance;
        Gizmos.DrawWireCube(endPosition, boxSize);

        // Narysuj linie ³¹cz¹ce rogi obu boxów
        Gizmos.color = Color.yellow;
        Vector3 topLeft = origin + new Vector3(-width / 2, height / 2, 0);
        Vector3 topRight = origin + new Vector3(width / 2, height / 2, 0);
        Vector3 bottomLeft = origin + new Vector3(-width / 2, -height / 2, 0);
        Vector3 bottomRight = origin + new Vector3(width / 2, -height / 2, 0);

        Vector3 endTopLeft = endPosition + new Vector3(-width / 2, height / 2, 0);
        Vector3 endTopRight = endPosition + new Vector3(width / 2, height / 2, 0);
        Vector3 endBottomLeft = endPosition + new Vector3(-width / 2, -height / 2, 0);
        Vector3 endBottomRight = endPosition + new Vector3(width / 2, -height / 2, 0);

        Gizmos.DrawLine(topLeft, endTopLeft);
        Gizmos.DrawLine(topRight, endTopRight);
        Gizmos.DrawLine(bottomLeft, endBottomLeft);
        Gizmos.DrawLine(bottomRight, endBottomRight);
    }

}


