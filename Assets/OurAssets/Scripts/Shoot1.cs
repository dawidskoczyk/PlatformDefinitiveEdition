using UnityEngine;

public class Shoot1 : MonoBehaviour
{
    [SerializeField] GameObject muzzle;

    private void Start()
    {
        var muz = Instantiate(muzzle, transform.position, Quaternion.identity);
        muz.transform.parent = transform;
        muz.GetComponent<ParticleSystem>().Play();
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Enemy") {
            print("trafi³o przeciwnika");
            Destroy(gameObject, 0.01f);
            collision.GetComponent<Hearts>().SubHp(1);
        }
        else
        {
            print(collision.transform.tag + "..+.." + collision.gameObject.layer);
            
            Destroy(gameObject, 2f);
        }
    }
}
