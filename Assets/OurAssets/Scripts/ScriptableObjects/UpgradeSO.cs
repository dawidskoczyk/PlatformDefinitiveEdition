using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Perks/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    public Sprite sprite;
    public string upgradeName;
    [TextArea]
    public string upgradeDescription;
    public float value;
}
