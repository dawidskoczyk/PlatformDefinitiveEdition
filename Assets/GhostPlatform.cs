using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPlatform : MonoBehaviour
{
    [SerializeField] float stableTime;
    [SerializeField] float flashTime;
    [SerializeField] float hiddingTime;

    TilemapRenderer tilemapRenderer;
    TilemapCollider2D tilemapCollider2D;

    float timer1;
    float timer2;
    float timer3;

    bool stableTimeFlies;
    bool flashTimeFlies;
    bool hiddingTimeFlies;


    void Start()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapCollider2D = GetComponent<TilemapCollider2D>();
    }

    void Update()
    {
        if (stableTimeFlies)
            timer1 += Time.deltaTime;
        if (flashTimeFlies)
            timer2 += Time.deltaTime;
        if (hiddingTimeFlies)
            timer3 += Time.deltaTime;

        if (timer1 >= stableTime)
        {
            stableTimeFlies = false;
            timer1 = 0;

            flashTimeFlies = true;
            StartCoroutine(PingingPlatform());
        }

        if(timer2 >= flashTime)
        {
            flashTimeFlies = false;
            timer2 = 0;

            hiddingTimeFlies = true;
            tilemapRenderer.enabled = false;
            tilemapCollider2D.enabled = false;
        }

        if(timer3 >= hiddingTime)
        {
            hiddingTimeFlies = false;
            timer3 = 0;

            tilemapRenderer.enabled = true;
            tilemapCollider2D.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player" && !flashTimeFlies && !hiddingTimeFlies)
        {
            print("Player-platform collision");

            stableTimeFlies = true;
        }
    }

    IEnumerator PingingPlatform()
    {
        while (flashTimeFlies)
        {
            tilemapRenderer.enabled = !tilemapRenderer.enabled;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
