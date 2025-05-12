using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerkButtonManager : MonoBehaviour
{
    public static List<UIPerkButton> PerkButtons;
    void Start()
    {
        PerkButtons = new List<UIPerkButton>();
        PerkButtons = GetComponentsInChildren<UIPerkButton>().ToList();
        print("lista "+ PerkButtons.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
