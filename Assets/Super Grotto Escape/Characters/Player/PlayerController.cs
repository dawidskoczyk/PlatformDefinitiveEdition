using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerState state;
    Animator animator;
    Rigidbody2D rb;
    bool isLocked = false;
    bool jumpReady = true;
    [SerializeField] float maxSpeed = 2f;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite jumpSprite;
    [SerializeField] Sprite fallSprite;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float smoothTime = 0.2f;
    int jumpCounter = 0;
    bool wallJump = false;
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
        if (isLocked) return;

        EvaluateState();

        switch (state)
        {
            case PlayerState.Idle:
                animator.Play("idle-Animation");
                rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
                StopAllCoroutines();
                break;
            case PlayerState.Run:
                StopAllCoroutines();
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
                StopAllCoroutines();
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
        rb.linearVelocity = new Vector2(0, 0);
        rb.gravityScale = 0;
        yield return new WaitForSeconds(0.5f);
        isLocked = false;
        rb.gravityScale = 1f;
        if (!GroundCheck()) ChangeCharacterState(PlayerState.Jump);
    }

    void Run()
    {
        animator.Play("Run_Animation");
        if (Input.GetKey(KeyCode.A) && GroundCheck()) rb.AddForce(new Vector2(-2f, 0), ForceMode2D.Force);
        if (Input.GetKey(KeyCode.D) && GroundCheck()) rb.AddForce(new Vector2(2f, 0), ForceMode2D.Force);

        Vector2 currentVelocity = rb.linearVelocity;
        Vector2 clampedVelocity = currentVelocity;
        clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed, maxSpeed);
        Vector2 lerpedVelocity = Vector2.Lerp(currentVelocity, clampedVelocity, smoothTime * Time.deltaTime);
        rb.linearVelocity = lerpedVelocity;
    }
    void Jump()
    {
        //jeszcze do przerobienia case kiedy postaæ biegnie, spada z platformy i dotyka œciany, bo wtedy siê jej nie ³apie bo ³apanie œciany jest w stanie skoku. \
        //Bêdzie trzeba ca³¹ obs³ugê œcian jakoœ przenieœæ albo obs³u¿yæ ten wyj¹tek kopiuj¹c fragment kodu do stanu run
        if (Input.GetKeyDown(KeyCode.Space) && GroundCheck() && jumpCounter == 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, 6f), ForceMode2D.Impulse);
            jumpCounter = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && jumpCounter == 1)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(0, 6f), ForceMode2D.Impulse);
            jumpCounter = 0; // Oznacza, ¿e wykorzystaliœmy oba skoki
        }
        else if (Input.GetKeyDown(KeyCode.Space) && WallCheck())
        {
            wallJump = true;
            rb.gravityScale = 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
            rb.AddForce(new Vector2(Input.GetAxis("Horizontal")*3f, 6f), ForceMode2D.Impulse);
            jumpCounter = 0; // Oznacza, ¿e wykorzystaliœmy oba skoki
            Invoke(nameof(ChangeJumpReady),0.2f);
        }

        if (!GroundCheck())
        {
            if (WallCheck() && !wallJump)
            {
                rb.gravityScale = 0.3f;
                rb.linearVelocity = new Vector2(0, 0);
            }
            else
            {
                rb.gravityScale = 1;
                rb.AddForce(new Vector2(Input.GetAxis("Horizontal") * 0.5f, 0), ForceMode2D.Force);

                Vector2 currentVelocity = rb.linearVelocity;
                Vector2 clampedVelocity = currentVelocity;
                clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -(maxSpeed * 0.5f), (maxSpeed * 0.5f));
                Vector2 lerpedVelocity = Vector2.Lerp(currentVelocity, clampedVelocity, smoothTime * Time.deltaTime);
                rb.linearVelocity = lerpedVelocity;
            }
        }

        //to nie dzia³a trzeba zrobiæ animacje
        if (rb.linearVelocity.y < 0)
            spriteRenderer.sprite = fallSprite;
        else if (rb.linearVelocity.y > 0)
            spriteRenderer.sprite = jumpSprite;
    }
    void EvaluateState()
    {
        if (!Input.anyKey && GroundCheck()) ChangeCharacterState(PlayerState.Idle);
        else if (Input.GetKeyDown(KeyCode.Mouse0)) ChangeCharacterState(PlayerState.Attack);
        else if (Input.GetKeyDown(KeyCode.Space)) ChangeCharacterState(PlayerState.Jump);
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && GroundCheck() && rb.linearVelocityY<1f) ChangeCharacterState(PlayerState.Run);
    }
    void ChangeCharacterState(PlayerState playerState)
    {
        state = playerState;
    }
    void ChangeJumpReady()
    {
        wallJump = false;
    }
    bool GroundCheck()
    {
        bool centerHit = Physics2D.Raycast(transform.position, Vector2.down, 0.65f, whatIsGround);

        bool leftHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y), Vector2.down, 0.65f, whatIsGround);

        bool leftHit1 = Physics2D.Raycast(new Vector2(transform.position.x - 0.15f, transform.position.y), Vector2.down, 0.65f, whatIsGround);

        bool rightHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.3f, transform.position.y), Vector2.down, 0.65f, whatIsGround);

        bool rightHit1 = Physics2D.Raycast(new Vector2(transform.position.x + 0.15f, transform.position.y), Vector2.down, 0.65f, whatIsGround);

        return centerHit || leftHit || rightHit || leftHit1 || rightHit1;
    }
    bool WallCheck()
    {
        bool centerHit = Physics2D.Raycast(transform.position, Vector2.right, 0.65f, whatIsGround);
        bool upHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.2f), Vector2.right, 0.65f, whatIsGround);
        bool downHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.2f), Vector2.right, 0.65f, whatIsGround);
        bool upHit1 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.2f), Vector2.left, 0.65f, whatIsGround);
        bool downHit1 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.2f), Vector2.left, 0.65f, whatIsGround);
        bool centerHit1 = Physics2D.Raycast(transform.position, Vector2.left, 0.65f, whatIsGround);
        return centerHit || upHit || downHit || centerHit1 || upHit1 || downHit1;
    }
    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector2.down * 0.65f, Color.red);
        Debug.DrawRay(new Vector2(transform.position.x - 0.3f, transform.position.y), Vector2.down * 0.65f, Color.green);
        Debug.DrawRay(new Vector2(transform.position.x + 0.3f, transform.position.y), Vector2.down * 0.65f, Color.blue);

        // Definiowanie kolorów dla promieni (zielony gdy nie ma kolizji, czerwony gdy jest kolizja)
        Color noHitColor = new Color(0, 1, 0, 0.5f); // Zielony pó³przezroczysty
        Color hitColor = new Color(1, 0, 0, 0.5f);   // Czerwony pó³przezroczysty

        // Prawe promienie
        DrawRayWithHitCheck(transform.position, Vector2.right * 0.65f, whatIsGround, noHitColor, hitColor);
        DrawRayWithHitCheck(new Vector2(transform.position.x, transform.position.y + 0.2f), Vector2.right * 0.65f, whatIsGround, noHitColor, hitColor);
        DrawRayWithHitCheck(new Vector2(transform.position.x, transform.position.y - 0.2f), Vector2.right * 0.65f, whatIsGround, noHitColor, hitColor);

        // Lewe promienie
        DrawRayWithHitCheck(transform.position, Vector2.left * 0.65f, whatIsGround, noHitColor, hitColor);
        DrawRayWithHitCheck(new Vector2(transform.position.x, transform.position.y + 0.2f), Vector2.left * 0.65f, whatIsGround, noHitColor, hitColor);
        DrawRayWithHitCheck(new Vector2(transform.position.x, transform.position.y - 0.2f), Vector2.left * 0.65f, whatIsGround, noHitColor, hitColor);
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
}
