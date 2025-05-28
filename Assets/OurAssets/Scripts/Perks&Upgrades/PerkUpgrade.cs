using UnityEngine;
using UnityEngine.EventSystems;

public class PerkUpgrade : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject player;
    [TextArea]
    [SerializeField] string upgradeName;
    CombatManager combatManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatManager = player.GetComponent<CombatManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        combatManager.LevelUp(upgradeName);
    }

}
