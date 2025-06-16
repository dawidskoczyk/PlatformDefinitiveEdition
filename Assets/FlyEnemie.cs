using UnityEngine;

public class FlyEnemie : MonoBehaviour
{
    
    [SerializeField]Transform player;
    [SerializeField]float speed;
    [SerializeField] float changeDirTime;

    Vector2 currentDir;
    Rigidbody2D rb;
    float timer;
    bool chasePlayer;

    Hearts enemyHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<Hearts>();
        enemyHealth.OnDeath += PlayDeathAnimation;
        currentDir = Vector2.one;

        timer = Random.Range(0, changeDirTime - 0.01f);
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeDirTime)
        {
            if (chasePlayer)
            {
                timer = 0;

                currentDir = player.position - transform.position;
            }
            else
            {
                timer = 1;

                currentDir = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            }
            chasePlayer = !chasePlayer;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = currentDir.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Floor")
        {
            currentDir = -currentDir;
        }
        else if(collision.tag == "Player")
        {
            print("player -1Hp");
        }
    }

    public void PlayDeathAnimation()
    {
        print("Wróg zgin¹³!");
        enemyHealth.OnDeath -= PlayDeathAnimation;
        gameObject.SetActive(false);
    }
}
