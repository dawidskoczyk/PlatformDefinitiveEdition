using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public float speed = 3f;
    public float Xleft;
    public float Xright;
    [SerializeField] bool startDirRight = true;
    [SerializeField] LayerMask playerLayer;

    private int direction;

    bool canChangeDir = true;

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
        

        if (transform.position.x < Xleft || transform.position.x > Xright)
        {
            direction *= -1;
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector2.right * direction * speed, ForceMode2D.Force);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (!(collision.gameObject.tag == "Player"))
    //    {
    //        direction *= -1;

    //        canChangeDir = false;

    //        print("kolizja");

    //        return;
    //    }
    //}

}
