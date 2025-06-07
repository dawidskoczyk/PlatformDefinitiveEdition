using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float explosionRadius = 2f;
    public float explosionForce = 500f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<ParticleSystem>().Play();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in hitColliders)
        {
            // Pomi� siebie
            if (collider.gameObject == gameObject) continue;

            // Sprawd� odleg�o�� do centrum wybuchu
            Vector2 direction = (collider.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, collider.transform.position);

            // Oblicz si�� na podstawie odleg�o�ci (bli�ej = wi�cej si�y)
            float forceMultiplier = 1f - (distance / explosionRadius);

            // Dodaj si�� je�li obiekt ma Rigidbody2D
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(direction * explosionForce * forceMultiplier, ForceMode2D.Impulse);
            }

            // Zadaj obra�enia je�li obiekt ma komponent zdrowia
            //to napisa� chat wi�c pewnie �le ale sam pomys�, �eby odpycha� i zadawa� obra�enia na podstawie odleg�o�ci od centrum wybuchu jest ciekawy
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);

            // Dodatkowe gizmo - wype�niony okr�g z przezroczysto�ci�
            Color fillColor = Color.red;
            fillColor.a = 0.2f;
            Gizmos.color = fillColor;
            Gizmos.DrawSphere(transform.position, explosionRadius);
        }
    }
}
