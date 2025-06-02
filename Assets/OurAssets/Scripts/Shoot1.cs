using UnityEngine;

public class Shoot1 : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy") {
            print("trafi³o przeciwnika");
        }
        else
        {
            print(collision.transform.tag + "..+.." + collision.gameObject.layer);
            
            Destroy(gameObject, 2f);
        }
    }
}
