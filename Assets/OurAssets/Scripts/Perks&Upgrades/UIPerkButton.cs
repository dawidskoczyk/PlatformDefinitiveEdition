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
        PerkButtonManager.Reset += Reset;
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

                        int neededPerks = 0;
                        foreach (var o in n.previousPerks)
                        {
                            if (o.up == true)
                            {
                                neededPerks++;
                            }
                        }
                        if(neededPerks == n.previousPerks.Length)
                            n.enableToUp = true;
                }
                }
        }
    }
    private void Reset()
    {
            up = false;
            image.color = Color.white;
        if (previousPerks.Length == 0) enableToUp = true;
            else enableToUp = false;
        foreach(var keys in CombatManager.Perks.Keys.ToList())
        {
            CombatManager.Perks[keys] = false;
        }
        foreach (var keys in CombatManager.PerksHandGun.Keys.ToList())
        {
            CombatManager.PerksHandGun[keys] = false;
        }
    }

}
