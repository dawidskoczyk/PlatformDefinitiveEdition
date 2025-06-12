using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public float speed = 3f;
    public int attackDMG = 1;
    public float Xleft;
    public float Xright;
    [SerializeField] bool startDirRight = true;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject player;

    private float timer;
    private int direction;

    bool canChangeDir = true;

    Rigidbody2D rb;

    [SerializeField] GameObject explosionPlayer;


    Hearts enemyHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (startDirRight)
            direction = 1;
        else
            direction = -1;

        enemyHealth = GetComponent<Hearts>();

        enemyHealth.OnDeath += PlayDeathAnimation;
    }

    void Update()
    {
        MoveLeftRight();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!(collision.gameObject.tag == "Player"))
        {
            direction *= -1;
            timer = 0;

            canChangeDir = false;
            Invoke(nameof(CanChangeTrue), 0.5f);

            return;
        }
            

        float playersX = collision.transform.position.x;
        float playersY = collision.transform.position.y;
        bool safeTp = false;

        int tpMax = 0;
        while (!safeTp && tpMax < 10000)
        {
            tpMax++;

            Vector2 newPos = new Vector2(Random.Range(playersX - 5, playersX + 5), Random.Range(playersY, playersY + 2));
            transform.position = newPos;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f, ~playerLayer);
            if(colliders.Length <= 0)
                safeTp = true;
            foreach (Collider2D collider in colliders)
                print(collider.name);
            
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


    void MoveLeftRight()
    {
        timer += Time.deltaTime;

        //if ((transform.position.x < Xleft || transform.position.x > Xright) && canChangeDir)
        //{
        //    direction *= -1;
        //    timer = 0;

        //    canChangeDir = false;
        //    Invoke(nameof(CanChangeTrue), 0.5f);
        //}

        //transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        rb.AddForce(Vector2.right * direction * speed);
        print(Vector2.right * direction * speed);
    }

    void MoveRandom()
    {

    }

    void TeleportSafe()
    {

    }

    bool CheckIfPlayerClose(float range)
    {
        if(Physics2D.OverlapCircleAll(transform.position, range, playerLayer).Length > 0)
        {
            return true;
        }
        else
            return false;
    }

    void CanChangeTrue()
    {
        canChangeDir = true;
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
