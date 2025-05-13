using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public float speed = 3f;
    public float Xleft;
    public float Xright;
    [SerializeField] bool startDirRight = true;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject player;

    private int direction;

    bool canChangeDir = true;
    bool canMove = true;

    Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (startDirRight)
            direction = 1;
        else
            direction = -1;
    }


    void Update()
    {

        if ((transform.position.x < Xleft || transform.position.x > Xright) && canChangeDir)
        {
            direction *= -1;

            canChangeDir = false;
            Invoke(nameof(CanChangeTrue), 1f);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            canMove = false;
            rb.AddForce(Vector2.one * 2, ForceMode2D.Impulse);

            Invoke(nameof(CanMoveNow), 0.5f);
        }

        if(Mathf.Abs(player.transform.position.x - transform.position.x) < 3.5f)
        {
            if (player.transform.position.x > transform.position.x)
                direction = 1;
            else
                direction = -1;
        }
        


    }

    private void FixedUpdate()
    {
        //rb.AddForce(Vector2.right * direction * speed, ForceMode2D.Force);

        if(canMove)
            rb.linearVelocity = Vector2.right * direction * speed;
    }


    void CanChangeTrue()
    {
        canChangeDir = true;
    }

    void CanMoveNow()
    {
        canMove = true;
    }

}
