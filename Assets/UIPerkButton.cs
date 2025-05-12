using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIPerkButton : MonoBehaviour
{
    Image image;
    [SerializeField] private bool enableToUp;
    [SerializeField] private bool up;
    public string namePerk;
    [SerializeField] UIPerkButton[] previousPerks = new UIPerkButton[0];
    [SerializeField] UIPerkButton[] nextPerks = new UIPerkButton[0];

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void LevelUp()
    {
        if (enableToUp)
        {
            image.color = Color.red;
            up = true;
            //gameObject.SetActive(false);
            foreach (var p in PerkButtonManager.PerkButtons)   
                {
                    print(p.nextPerks);
                foreach(var n in p.nextPerks)
                {
                    if (n.previousPerks.Length <= 1)
                        n.enableToUp = true;
                    else
                    {
                        n.enableToUp = true;
                         // it may gives a troubles if yes add additional parameter when break loop below
                        foreach (var o in n.previousPerks)
                        {
                            if (o.up != true)
                                n.enableToUp = false;
                                break;
                        }
                    }

                }
                }
        }
    }
}
