using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour
{
    [SerializeField] List<UpgradeSO> upgradeList;
    [SerializeField] List<GameObject> options; 

    private int[] drawnValues;
    void Start()
    {
        drawnValues = new int[3];
        //upgradeList = new List<UpgradeSO>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Choose()
    {
       for (int i = 0; i < drawnValues.Length; i++)
        {
            drawnValues[i] = Random.Range(0,upgradeList.Count-1);

            options[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = upgradeList[drawnValues[i]].sprite;
            options[i].gameObject.transform.GetChild(1).GetComponent<TextMeshPro>().text = upgradeList[drawnValues[i]].upgradeDescription;
            upgradeList.RemoveAt(drawnValues[i]);
        }

        
    }
}
