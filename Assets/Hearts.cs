using UnityEngine;
using System;

public class Hearts : MonoBehaviour
{
    [SerializeField][Range(1,10)] int maxHp;
    [SerializeField] int currentHp;
    public GameObject kontener;
    public event Action OnDeath;


    void Start()
    {
        if (kontener != null)
        {
            kontener.GetComponent<RectTransform>().sizeDelta = new Vector2(45 * maxHp, 50);
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
        if (currentHp > 0)
        {
            currentHp -= hp;
        }
        else
        {
            OnDeath?.Invoke();
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
}
