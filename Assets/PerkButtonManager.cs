using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerkButtonManager : MonoBehaviour
{
    public static List<UIPerkButton> PerkButtons;
    public static event Action Reset;
    [SerializeField] private GameObject panel;
    [SerializeField] GameObject panel3;
    void Start()
    {
        PerkButtons = new List<UIPerkButton>();
        PerkButtons = GetComponentsInChildren<UIPerkButton>().ToList();
        //print("lista "+ PerkButtons.Count);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) 
        {
            panel.SetActive(!panel.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.O))
            panel3.SetActive(!panel3.activeSelf);
    }
}
