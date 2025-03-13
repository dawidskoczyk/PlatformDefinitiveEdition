using UnityEngine;

public class TakingDamage : MonoBehaviour
{
    public GameObject player;

    [SerializeField] float power;
    [SerializeField] float powerUp;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage();
        }

        if (Input.GetKeyUp(KeyCode.U))
        {
            TakeUpDamage();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Wall")
        {
            print("stunned");
        }
    }

    void TakeDamage()
    {
        Vector2 pushDir = transform.position - player.transform.position;
        rb.AddForce(pushDir.normalized * power);
    }

    void TakeUpDamage()
    {
        float pushDir = transform.position.x - player.transform.position.x;

        //if(transform.position.x < player.transform.position.x)
        //{
        //    pushDir = -1;
        //}

        rb.AddForce(new Vector2(pushDir, 5) * powerUp);
    }
}
