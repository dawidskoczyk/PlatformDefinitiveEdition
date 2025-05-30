using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour
{
    [SerializeField] List<UpgradeSO> upgradeList;
    [SerializeField] List<UpgradeSO> upgradeListCopy;
    [SerializeField] List<GameObject> options;
    [SerializeField] GameObject panel;

    private int[] drawnValues;
    void Start()
    {
        drawnValues = new int[3];
        for(int i = 0; i< 3; i++ )
            drawnValues[i] = i;
        upgradeListCopy = new List<UpgradeSO>();

        foreach( UpgradeSO upgrade in upgradeList )
            upgradeListCopy.Add( upgrade );
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
            Draw();
    }
    void Draw()
    {
       for (int i = 0; i < drawnValues.Length; i++)
        {
            drawnValues[i] = Random.Range(0,upgradeList.Count-1);

            options[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = upgradeList[drawnValues[i]].sprite;
            options[i].gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = upgradeList[drawnValues[i]].upgradeDescription;
            upgradeList.RemoveAt(drawnValues[i]);
        }
        upgradeList.Clear();
        foreach (UpgradeSO upgrade in upgradeListCopy)
            upgradeList.Add(upgrade);

    }
}
