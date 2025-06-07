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
            // Pomiñ siebie
            if (collider.gameObject == gameObject) continue;

            // SprawdŸ odleg³oœæ do centrum wybuchu
            Vector2 direction = (collider.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, collider.transform.position);

            // Oblicz si³ê na podstawie odleg³oœci (bli¿ej = wiêcej si³y)
            float forceMultiplier = 1f - (distance / explosionRadius);

            // Dodaj si³ê jeœli obiekt ma Rigidbody2D
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(direction * explosionForce * forceMultiplier, ForceMode2D.Impulse);
            }

            // Zadaj obra¿enia jeœli obiekt ma komponent zdrowia
            //to napisa³ chat wiêc pewnie Ÿle ale sam pomys³, ¿eby odpychaæ i zadawaæ obra¿enia na podstawie odleg³oœci od centrum wybuchu jest ciekawy
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);

            // Dodatkowe gizmo - wype³niony okr¹g z przezroczystoœci¹
            Color fillColor = Color.red;
            fillColor.a = 0.2f;
            Gizmos.color = fillColor;
            Gizmos.DrawSphere(transform.position, explosionRadius);
        }
    }
}
