using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Collections;

public class Hearts : MonoBehaviour
{
    [SerializeField][Range(1,10)] int maxHp;
    [SerializeField] int currentHp;
    public GameObject kontener;
    public event Action OnDeath;
    bool isInvincible;
    [SerializeField] float invincibleTime;
    float timer;
    SpriteRenderer spriteRenderer;
    [SerializeField] float pingFrequency;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (kontener != null)
        {
            //kontener.GetComponent<RectTransform>().sizeDelta = new Vector2(45 * maxHp, 50);
            RefreshUI();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            AddHp(1);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            SubHp(1);
        }
    }

    public void AddHp(int hp)
    {
        if (currentHp < maxHp)
        {
            currentHp += hp;
        }

        print("hp teraz:  " + currentHp);
        //RefreshUI();
    }

    public void SubHp(int hp)
    {
        if (isInvincible)
            return;


        currentHp -= hp;
        if (currentHp > 0)
        {
            kontener.transform.GetChild(currentHp).gameObject.SetActive(false);
        }
        else
        {
            kontener.transform.GetChild(0).gameObject.SetActive(false);
            OnDeath?.Invoke();
        }
        if(gameObject.tag == "Player")
        {
            isInvincible = true;
            StartCoroutine(PingingPlayer());
            Invoke(nameof(InvincibleEnd), invincibleTime);
        }
        print("hp teraz:  " + currentHp);
        //RefreshUI();
    }

    void RefreshUI()
    {
        for (int i = 0; i < maxHp; i++)
        {
            if (i < currentHp)
            {
                kontener.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                kontener.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    IEnumerator PingingPlayer()
    {
        float newPingF = pingFrequency;
        spriteRenderer.color = Color.white;

        while (isInvincible)
        {

            if (spriteRenderer.color == Color.white)
            {
                spriteRenderer.color = Color.black;
                yield return new WaitForSeconds(newPingF);
            }
            else
            {
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(newPingF * 2);
            }

            newPingF -= newPingF/20;
        }

        spriteRenderer.color = Color.white;
    }

    void InvincibleEnd()
    {
        isInvincible = false;
    }
}
