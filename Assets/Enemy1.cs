using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public float speed = 3f;
    public float Xleft;
    public float Xright;
    [SerializeField] bool startDirRight = true;
    [SerializeField] LayerMask playerLayer;

    private float timer;
    private int direction;

    bool canChangeDir = true;



    void Start()
    {
        if (startDirRight)
            direction = 1;
        else
            direction = -1;
    }

    void Update()
    {
        MoveLeftRight();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("kolizja1");
        if(!(collision.gameObject.tag == "Player"))
        {
            direction *= -1;
            timer = 0;

            canChangeDir = false;
            Invoke(nameof(CanChangeTrue), 0.5f);
            print("kolizja");

            return;
        }
            

        float playersX = collision.transform.position.x;
        float playersY = collision.transform.position.y;
        bool safeTp = false;

        int tpMax = 0;
        while (!safeTp && tpMax < 10000)
        {
            tpMax++;

            Vector2 newPos = new Vector2(Random.Range(playersX - 5, playersX + 5), Random.Range(playersY, playersY + 5));
            transform.position = newPos;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            if(colliders.Length <= 0)
                safeTp = true;
            foreach (Collider2D collider in colliders)
                print(collider.name);
            
        }
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

        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
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
}
