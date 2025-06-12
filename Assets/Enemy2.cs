using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    enum Enemy1state { Idle, Attack, Die}
    [SerializeField]Enemy1state currentState;
    Enemy1state previousState;
    public float speed = 3f;
    public float attackRange = 3f;
    public int attackDMG = 1;
    public float Xleft;
    public float Xright;
    [SerializeField] bool startDirRight = true;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject player;

    [SerializeField] GameObject explosionPlayer;

    private int direction;

    bool canChangeDir = true;
    bool canMove = true;

    Rigidbody2D rb;

    Hearts enemyHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        enemyHealth = GetComponent<Hearts>();

        currentState = Enemy1state.Idle;
        previousState = Enemy1state.Idle;

        enemyHealth.OnDeath += PlayDeathAnimation;

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
            
            // spr w ktora strone isc:
            if ((transform.position.x < Xleft || transform.position.x > Xright) && canChangeDir)
            {
                direction *= -1;
                speed = 1f;
                Invoke(nameof(NormalSpeed), 1f);

                canChangeDir = false;
                Invoke(nameof(CanChangeTrue), 1f);
            }

            // idle movement:
            if (canMove)
                rb.linearVelocity = Vector2.right * direction * speed;
        }
        

        if (currentState == Enemy1state.Attack)
        {
            if(previousState == Enemy1state.Idle)
                StartCoroutine(AttackPlayer());
        }

        previousState = currentState;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag != "Player")
        {
            if(currentState == Enemy1state.Idle)
            {
                direction *= -1;
            }
        }

        rb.constraints = RigidbodyConstraints2D.FreezePositionX;

        if (collision.transform.tag == "Player")
        {
            player.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position) * 100f, ForceMode2D.Impulse);
            player.GetComponent<Hearts>().SubHp(attackDMG);
            //po tym gracz musi przechodziæ w stan getDamage gdzie stanie siê nietykalny przez chwile lub dostanie stuna
        }
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //to dzia³a œrednio, bo player musi byæ stunowany na chwile, ¿eby nie da³o sie nim biec podczas odepchniêcia
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

    public void PlayDeathAnimation()
    {
        print("Wróg zgin¹³!");
        enemyHealth.OnDeath -= PlayDeathAnimation;
        gameObject.SetActive(false);

        var exp = Instantiate(explosionPlayer, transform.position, Quaternion.identity);
        exp.GetComponent<ParticleSystem>().Play();
    }

}
