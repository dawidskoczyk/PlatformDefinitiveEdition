using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] GameObject activeWeapon; //aktywna bron lewy przycisk
    [SerializeField] float xp; 
    [SerializeField] float dmg; 
        
    [SerializeField] bool critChance;
    [SerializeField] float critDmg;
    [SerializeField] bool rangeSlash;
    [SerializeField] bool elementalEffect;

    [SerializeField] float critChanceValue;
    [SerializeField] float rangeSlashValue;

    [SerializeField] bool fireEffect;
    [SerializeField] bool iceEffect;
    [SerializeField] bool poisonEffect;

    [SerializeField] bool fireEffectTick;
    [SerializeField] bool iceEffectTick;
    [SerializeField] bool poisonEffectTick;

    public static Dictionary<string, bool> Perks;
    public static Dictionary<string, bool> Upgrades; //klasa chyba lepsza b�dzie od s�ownik
    private void Start()
    {
        Perks = new Dictionary<string, bool>
        {
            { "critChance", false },
            { "rangeSlash", false },
            { "elementalEffect", false },
            { "CritDmg", false },
            { "elementalRange", false }
        };
        Upgrades = new Dictionary<string, bool>
        {
            { "critChance+", false },
            { "rangeSlashDouble", false },
            { "elementalEffect+", false }, //to później zrobimy te elementale
            { "critDmg+", false },
            { "hp+", false },
            { "gold+", false },
            { "as+", false }, // większa prędkość ataku
            { "ad+", false }, //więcej obrażeń
            { "slam+", false },//fala po użyciu slama (wcisnięcie s podczas skoku)
            { "dash+", false}
        };
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
            LevelUp("critChance");
        if(Input.GetKeyUp(KeyCode.Alpha2))
            LevelUp("rangeSlash");
        if(Input.GetKeyUp(KeyCode.Alpha3))
            LevelUp("elementalEffect");
    }

    public void LevelUp(string perk)
    {
        Perks[perk] = true;
        
    } 
    public void Reset(string perk)
    {
        Perks[perk] = false;
    }
}

//public class perk
//{
//    public string name;
//    public bool unlocked;
//    public list<string> dependencies;

//    public perk(string name, list<string> dependencies = null)
//    {
//        name = name;
//        unlocked = false;
//        dependencies = dependencies ?? new list<string>();
//    }

//    public bool canunlock(dictionary<string, perk> allperks)
//    {
//        return dependencies.all(dep => allperks[dep].unlocked);
//    }
//}

