using System.Collections;
using UnityEngine;

public abstract class IAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public abstract IEnumerator Attack(PlayerControllerSM player);

}
