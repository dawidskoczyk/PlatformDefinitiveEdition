using UnityEngine;

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

    //[SerializeField] float maxSpeed = 2f;
    //[SerializeField] float jumpPower = 6f;
    //[SerializeField] float distanceToWall = 0.5f;
    //[SerializeField] float groundXSize = 0.6f;

    //[SerializeField] LayerMask whatIsGround;

    //[SerializeField] int jumpCounter = 0;
    //[SerializeField] public bool isLocked = false;
    //[SerializeField] bool jump = false;
    //[SerializeField] float horizontalInput;
    //[SerializeField] bool doubleJumpAfterWall;
    //[SerializeField] float boxCastXMiniOffset;
    //[SerializeField] public GameObject shoot1;
    //[SerializeField] public Transform gunSpot;
    //[SerializeField] public float shootSpeed;
    //[SerializeField] public float dashForce;
    //[SerializeField] public bool canAttack = false;
    //[SerializeField] bool dashAttack = false;
    //[SerializeField] float dashBrakeTime = 1;
    //[SerializeField] float chargeTimer = 0f;
    //[SerializeField] public bool slam;
    //[SerializeField] public bool upAttack;
    //[SerializeField] public bool rightClick;

    //bool canWallSlide = true;
    //bool wallJump = false;
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
    }

    void TakeInput()
    {
        //if(!canAttack)
        //    canAttack = Input.GetKeyDown(KeyCode.Mouse0);
        //if (!dashAttack)
        //    dashAttack = Input.GetKeyDown(KeyCode.LeftShift);
        //if (!jump) // -> czemu? podczas skoku przecie¿ mo¿na skakaæ ponownie
        //    jump = Input.GetKeyDown(KeyCode.Space);
        //if (!slam)
        //    slam = Input.GetKeyDown(KeyCode.S);
        //if (!upAttack)
        //    upAttack = Input.GetKeyDown(KeyCode.W);

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
        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
    }

    public bool IsGrounded()
    {
        // Placeholder – zamieñ na Raycast/Overlap
        return rb.linearVelocityY == 0;
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

    public float GetHorizontalInput() => horizontalInput;
    public bool IsJumpPressed() => jumpPressed;
    public bool IsDashPressed() => dashPressed;
    public bool IsAttackPressed() => attackPressed;

    public Rigidbody2D GetRigidbody() => rb;
    public Animator GetAnimator() => animator;
    public StateMachine GetStateMachine() => stateMachine;
}
