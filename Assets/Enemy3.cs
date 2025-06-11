using UnityEngine;
using System.Collections;

public class Enemy3 : MonoBehaviour
{
    enum Enemy1state { Idle, Attack, Die, GetDamage }
    [SerializeField] Enemy1state currentState;
    Enemy1state previousState;
    public float speed = 3f;
    public float attackRange = 3f;
    public float Xleft;
    public float Xright;
    //[SerializeField] bool startDirRight = true;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject player;
    [SerializeField] GameObject explosionPlayer;

    private int direction;

    Vector2 attackDir;

    bool canChangeDir = true;
    bool canAttack = true;

    Rigidbody2D rb;

    Hearts enemyHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentState = Enemy1state.Idle;
        previousState = Enemy1state.Idle;
        enemyHealth = GetComponent<Hearts>();

        enemyHealth.OnDeath += PlayDeathAnimation;

    }


    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    canMove = false;
        //    rb.AddForce(Vector2.one * 2, ForceMode2D.Impulse);

        //    Invoke(nameof(CanMoveNow), 0.5f);
        //}

        if (Mathf.Abs((player.transform.position - transform.position).magnitude) < attackRange && canAttack)
        {
            canAttack = false;
            Invoke(nameof(CanAttackNow), 4f);

            currentState = Enemy1state.Attack;
            Invoke(nameof(IdleState), 0.2f);

            attackDir = (player.transform.position - transform.position).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (currentState == Enemy1state.Attack)
        {
            rb.linearVelocity = attackDir * speed;

        }
            // sprawdzanie stanu:
            //if (Mathf.Abs((player.transform.position - transform.position).magnitude) < attackRange)
            //{
            //    currentState = Enemy1state.Attack;
            //}
            //else
            //{
            //    currentState = Enemy1state.Idle;
            //}


            //if (currentState == Enemy1state.Idle)
            //{
            //    if (previousState == Enemy1state.Attack)
            //    {
            //        StopCoroutine(AttackPlayer());
            //        print("stop attackaaa");
            //    }
            //}

            //if (currentState == Enemy1state.Attack)
            //{
            //    if (previousState == Enemy1state.Idle)
            //        StartCoroutine(AttackPlayer());
            //}
            //else
            //    print("WTF state");

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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //GetDamage(1);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;

        if (collision.transform.tag == "Player")
        {
            player.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position) * 100f, ForceMode2D.Impulse);
        }
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //to dzia³a œrednio, bo player musi byæ stunowany na chwile, ¿eby nie da³o sie nim biec podczas odepchniêcia
    }


    void CanChangeTrue()
    {
        canChangeDir = true;
    }

    void CanAttackNow()
    {
        canAttack = true;
    }

    void IdleState()
    {
        currentState = Enemy1state.Idle;
    }
    public void GetDamage(int valueDMG)
    {
        currentState = Enemy1state.GetDamage;
        GetComponent<Hearts>().SubHp(valueDMG);
    }

    void NormalSpeed()
    {
        speed = 3;
    }
    public void PlayDeathAnimation()
    {
        print("Wróg zgin¹³!");
        enemyHealth.OnDeath -= PlayDeathAnimation;

        var exp = Instantiate(explosionPlayer, transform.position, Quaternion.identity);
        exp.GetComponent<ParticleSystem>().Play();

        gameObject.SetActive(false);
    }
}
