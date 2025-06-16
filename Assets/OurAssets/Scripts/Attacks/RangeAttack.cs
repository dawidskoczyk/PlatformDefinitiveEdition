using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class RangeAttack : IAttack
{
    [SerializeField] private GameObject grenade;
    [SerializeField] private GameObject shoot1;
    [SerializeField] private float shootSpeed;
    [SerializeField] private GameObject muzzle;
    [SerializeField] private float fireRate;
   public override IEnumerator Attack( PlayerControllerSM player)
    {
        //player.isLocked = true;
        //print("attack");
        player.GetAnimator().Play("Shoot_Animation");
        Vector2 savedVelocity = player.GetRigidbody().linearVelocity;
        player.GetRigidbody().linearVelocity = Vector2.zero;
        player.GetRigidbody().gravityScale = 0;


        //strza³ ale zmienne bêdzie trzeba uporz¹dkowaæ jak to zacznie dzia³aæ, czyli przenieœæ Serialize Fieldy z playera do broni
        Vector2 shotPosition = player.spriteRenderer.flipX ? transform.TransformPoint(new Vector2(-player.gunSpot.localPosition.x, player.gunSpot.localPosition.y)) : player.gunSpot.position;
        
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        Vector2 shotDir = mousePosition - shotPosition;

        float angle = Mathf.Atan2(shotDir.y, shotDir.x) * Mathf.Rad2Deg;

        if (shotDir.x < 0)
            player.spriteRenderer.flipX = true;
        else
            player.spriteRenderer.flipX = false;

        Quaternion rotation = player.spriteRenderer.flipX ? Quaternion.Euler(0, 0, angle + 180f) : Quaternion.Euler(0, 0, angle);
        yield return new WaitForSeconds(fireRate);
        shotDir = mousePosition - shotPosition;

        //angle = Mathf.Atan2(shotDir.y, shotDir.x) * Mathf.Rad2Deg;
        print("3shots " + CombatManager.PerksHandGun["3shots"]);
        if (CombatManager.PerksHandGun["3grenade"] && player.rightClick)
        {
            player.rightClick = false;
            StartCoroutine(grenade3Attack(shoot1, shotPosition, rotation, shotDir, player));
        }
        else if (CombatManager.PerksHandGun["grenade"] && player.rightClick)
        {
            player.rightClick = false;
            grenadeAttack(shoot1, shotPosition, rotation, shotDir, player);
        } 
        else
        {
            if (CombatManager.PerksHandGun["shotgun"])
                shotgunAttack(shoot1, shotPosition, rotation, shotDir, player);
            else if (CombatManager.PerksHandGun["3shots"])
                StartCoroutine(Shoot3Attack(shoot1, shotPosition, rotation, shotDir, player));
            else
                BaseAttack(shoot1, shotPosition, rotation, shotDir.normalized, player);
        }
        player.GetRigidbody().gravityScale = 1;
        player.GetRigidbody().linearVelocity = savedVelocity;

        player.GetStateMachine().attackState.lockedState = false;
    }

    private void BaseAttack(GameObject shotPrefab, Vector2 shootPos, Quaternion shotRotation, Vector2 shotDir, PlayerControllerSM player)
    {
        GameObject sh1 = Instantiate(shotPrefab, shootPos, shotRotation);
        PlayMuzzle(muzzle, shootPos);

        sh1.GetComponent<Rigidbody2D>().AddForce(shotDir * shootSpeed, ForceMode2D.Impulse);
        if (player.spriteRenderer.flipX) sh1.GetComponent<SpriteRenderer>().flipX = true;
    }
    private IEnumerator Shoot3Attack(GameObject shootPrefab, Vector2 shootPos, Quaternion shootRotation, Vector2 shootDir, PlayerControllerSM player)
    {
        GameObject sh1 = Instantiate(shootPrefab, shootPos, shootRotation);
        PlayMuzzle(muzzle, shootPos);
        GameObject sh2 = Instantiate(shootPrefab, shootPos + new Vector2(0,0.01f), shootRotation);
        sh2.SetActive(false);
        GameObject sh3 = Instantiate(shootPrefab, shootPos - new Vector2(0, 0.01f), shootRotation); 
        sh3.SetActive(false);
        
        sh1.GetComponent<Rigidbody2D>().AddForce(shootDir * shootSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        PlayMuzzle(muzzle, shootPos);
        sh2.SetActive(true);
        sh2.GetComponent<Rigidbody2D>().AddForce(shootDir * shootSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        PlayMuzzle(muzzle, shootPos);
        sh3.SetActive(true);
        sh3.GetComponent<Rigidbody2D>().AddForce(shootDir * shootSpeed, ForceMode2D.Impulse);
        if (player.spriteRenderer.flipX)
        {
            sh1.GetComponent<SpriteRenderer>().flipX = true;
            sh2.GetComponent<SpriteRenderer>().flipX = true;
            sh3.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    private void shotgunAttack(GameObject shotPrefab, Vector2 shootPos, Quaternion shotRotation, Vector2 shotDir, PlayerControllerSM player)
    {
        GameObject[] sh = new GameObject[9];
        PlayMuzzle(muzzle, shootPos);
        for (int i =0; i<=9; i++)
        {
            float range = ((i / 100) - 0.05f);
            GameObject sh1 = Instantiate(shotPrefab, shootPos + new Vector2(0, range), shotRotation);
            //h[i] = sh1;
            sh1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            sh1.GetComponent<Rigidbody2D>().AddForce(shotDir * shootSpeed*0.5f, ForceMode2D.Impulse);
            if (player.spriteRenderer.flipX) sh1.GetComponent<SpriteRenderer>().flipX = true;
        }

    }

    private void grenadeAttack(GameObject shotPrefab, Vector2 shootPos, Quaternion shotRotation, Vector2 shotDir, PlayerControllerSM player)
    {
        GameObject sh1 = Instantiate(grenade, shootPos, shotRotation);
        PlayMuzzle(muzzle, shootPos);
        sh1.GetComponent<Rigidbody2D>().gravityScale = 1;
        sh1.GetComponent<Rigidbody2D>().AddForce(shotDir * shootSpeed*0.2f, ForceMode2D.Impulse);
        if (player.spriteRenderer.flipX) sh1.GetComponent<SpriteRenderer>().flipX = true;

        Destroy(sh1,3f);
    }   
    private IEnumerator grenade3Attack(GameObject shotPrefab, Vector2 shootPos, Quaternion shotRotation, Vector2 shotDir, PlayerControllerSM player)
    {
        GameObject sh1 = Instantiate(grenade, shootPos, shotRotation);
        GameObject sh2 = Instantiate(grenade, shootPos, shotRotation);
        sh2.SetActive(false);
        GameObject sh3 = Instantiate(grenade, shootPos, shotRotation);
        sh3.SetActive(false);

        sh1.GetComponent<Rigidbody2D>().gravityScale = 1; 
        sh2.GetComponent<Rigidbody2D>().gravityScale = 1; 
        sh3.GetComponent<Rigidbody2D>().gravityScale = 1;

        PlayMuzzle(muzzle, shootPos);
        sh1.GetComponent<Rigidbody2D>().AddForce(shotDir * shootSpeed*0.2f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        PlayMuzzle(muzzle, shootPos);
        sh2.SetActive(true);
        sh2.GetComponent<Rigidbody2D>().AddForce(shotDir * shootSpeed*0.3f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        PlayMuzzle(muzzle, shootPos);
        sh3.SetActive(true);
        sh3.GetComponent<Rigidbody2D>().AddForce(shotDir * shootSpeed*0.4f, ForceMode2D.Impulse);
        
        if (player.spriteRenderer.flipX) sh1.GetComponent<SpriteRenderer>().flipX = true;

        Destroy(sh1,3f);
        Destroy(sh2,3f);
        Destroy(sh3,3f);
    }
    void PlayMuzzle(GameObject muzzle, Vector2 shootPos)
    {
        GameObject muz = Instantiate(muzzle, shootPos, Quaternion.identity);
        muz.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        muz.transform.SetParent(transform);
        muz.GetComponent<ParticleSystem>().Play();

        Destroy(muz, 0.1f);
    }
}
