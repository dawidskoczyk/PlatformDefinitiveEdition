using UnityEngine;
using System.Collections;

public class Enemy3 : MonoBehaviour
{
    enum Enemy1state { Idle, Attack, Die }
    [SerializeField] Enemy1state currentState;
    Enemy1state previousState;
    public float speed = 3f;
    public float attackRange = 3f;
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

        currentState = Enemy1state.Idle;
        previousState = Enemy1state.Idle;

        if (startDirRight)
            direction = 1;
        else
            direction = -1;
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            canMove = false;
            rb.AddForce(Vector2.one * 2, ForceMode2D.Impulse);

            Invoke(nameof(CanMoveNow), 0.5f);
        }
    }

    private void FixedUpdate()
    {
        // sprawdzanie stanu:
        if (Mathf.Abs((player.transform.position - transform.position).magnitude) < attackRange)
        {
            currentState = Enemy1state.Attack;
        }
        else
        {
            currentState = Enemy1state.Idle;
        }


        if (currentState == Enemy1state.Idle)
        {
            if (previousState == Enemy1state.Attack)
            {
                StopCoroutine(AttackPlayer());
                print("stop attackaaa");
            }
        }

        if (currentState == Enemy1state.Attack)
        {
            if (previousState == Enemy1state.Idle)
                StartCoroutine(AttackPlayer());
        }
        else
            print("WTF state");

        previousState = currentState;
    }

    IEnumerator AttackPlayer()
    {
        while (currentState == Enemy1state.Attack)
        {
            if (player.transform.position.x - transform.position.x > 0)
            {
                rb.AddForce(new Vector2(1, 2) * 1, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(new Vector2(-1, 2) * 1, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(0.6f);
        }
    }


    void CanChangeTrue()
    {
        canChangeDir = true;
    }

    void CanMoveNow()
    {
        canMove = true;
    }

    void NormalSpeed()
    {
        speed = 3;
    }
}
