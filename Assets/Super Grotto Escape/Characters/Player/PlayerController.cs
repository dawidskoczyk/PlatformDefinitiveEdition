using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerState state;
    Animator animator;
    Rigidbody2D rb;
    bool isLocked = false;
    [SerializeField] float maxSpeed = 5f;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite jumpSprite;
    [SerializeField] Sprite fallSprite;
    [SerializeField] LayerMask whatIsGround;
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
        yield return new WaitForSeconds(2f);
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
        spriteRenderer.sprite = jumpSprite;
        if (Input.GetKey(KeyCode.Space) && GroundCheck()) rb.AddForce(new Vector2(rb.linearVelocityX, 0.5f)* 0.5f, ForceMode2D.Impulse);
        if (rb.linearVelocityY <0) spriteRenderer.sprite = fallSprite;
    }
    void EvaluateState()
    {
        if (!Input.anyKey) ChangeCharacterState(PlayerState.Idle);
        else if (Input.GetKeyDown(KeyCode.Mouse0)) ChangeCharacterState(PlayerState.Attack);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) ChangeCharacterState(PlayerState.Run);
        else if (Input.GetKeyDown(KeyCode.Space)) ChangeCharacterState(PlayerState.Jump);
    }
    void ChangeCharacterState(PlayerState playerState)
    {
        state = playerState;
    }
    bool GroundCheck()
    {
       return Physics2D.Raycast(transform.position, new Vector2 (0,-1), 0.65f , whatIsGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, Vector2.down);
    }
}
