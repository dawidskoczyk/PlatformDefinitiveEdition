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
    [SerializeField] float maxSpeed = 4f;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite jumpSprite;
    [SerializeField] Sprite fallSprite;
    [SerializeField] LayerMask whatIsGround;
    int jumpCounter = 0;
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
        yield return new WaitForSeconds(0.5f);
        isLocked = false;
    }

    void Run()
    {
        animator.Play("Run_Animation"); 
        if (Input.GetKey(KeyCode.A) && GroundCheck()) rb.AddForce(new Vector2(-4f,0), ForceMode2D.Force);
        if (Input.GetKey(KeyCode.D) && GroundCheck()) rb.AddForce(new Vector2( 4f,0), ForceMode2D.Force);

        Vector2 clampedVelocity = rb.linearVelocity;
        clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed, maxSpeed);
        rb.linearVelocity = clampedVelocity;
    }
    void Jump()
    {
            if (Input.GetKeyDown(KeyCode.Space) && GroundCheck() && jumpCounter == 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
                rb.AddForce(new Vector2(0, 5f), ForceMode2D.Impulse);
                jumpCounter = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && jumpCounter == 1)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zerujemy prêdkoœæ pionow¹
                rb.AddForce(new Vector2(0, 5f), ForceMode2D.Impulse);
                jumpCounter = 0; // Oznacza, ¿e wykorzystaliœmy oba skoki
            }

        if (!GroundCheck())
        {
            rb.AddForce(new Vector2(Input.GetAxis("Horizontal"), 0),ForceMode2D.Force);
            Vector2 clampedVelocity = rb.linearVelocity;
            clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxSpeed, maxSpeed);
            rb.linearVelocity = clampedVelocity;
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
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && GroundCheck()) ChangeCharacterState(PlayerState.Run);
    }
    void ChangeCharacterState(PlayerState playerState)
    {
        state = playerState;
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
    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector2.down * 0.65f, Color.red);
        Debug.DrawRay(new Vector2(transform.position.x - 0.3f, transform.position.y), Vector2.down * 0.65f, Color.green);
        Debug.DrawRay(new Vector2(transform.position.x + 0.3f, transform.position.y), Vector2.down * 0.65f, Color.blue);
    }
    void ChangeJumpReady()
    {
        jumpReady = !jumpReady;   
    }
}
