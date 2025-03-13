using UnityEngine;

public class Hearts : MonoBehaviour
{
    [SerializeField][Range(1,10)] int maxHp;
    [SerializeField] int currentHp;
    public GameObject kontener;

    void Start()
    {
        kontener.GetComponent<RectTransform>().sizeDelta = new Vector2(45 * maxHp, 50);
        for (int i = 0; i<maxHp; i++)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
