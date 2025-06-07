using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class RangeAttack : IAttack
{
    [SerializeField] private GameObject grenade;
   public override IEnumerator Attack( PlayerController player)
    {
        player.isLocked = true;
        print("attack");
        player.animator.Play("Shoot_Animation");
        Vector2 savedVelocity = player.rb.linearVelocity;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.gravityScale = 0;
        yield return new WaitForSeconds(0.3f);

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

        print("3shots " + CombatManager.PerksHandGun["3shots"]);
        if (CombatManager.PerksHandGun["grenade"] && player.rightClick)
        {
            player.rightClick = false;
            grenadeAttack(player.shoot1, shotPosition, rotation, shotDir, player);
        } 
        else
        {
            if (CombatManager.PerksHandGun["shotgun"])
                shotgunAttack(player.shoot1, shotPosition, rotation, shotDir, player);
            else if (CombatManager.PerksHandGun["3shots"])
                StartCoroutine(Shoot3Attack(player.shoot1, shotPosition, rotation, shotDir, player));
            else
                BaseAttack(player.shoot1, shotPosition, rotation, shotDir, player);
        }
        player.rb.gravityScale = 1;
        player.rb.linearVelocity = savedVelocity;

        player.isLocked = false;

        if (!player.GroundCheck()) player.ChangeCharacterState(PlayerController.PlayerState.Jump);
        else player.ChangeCharacterState(PlayerController.PlayerState.Idle);

        player.canAttack = false;
    }

    private void BaseAttack(GameObject shotPrefab, Vector2 shootPos, Quaternion shotRotation, Vector2 shotDir, PlayerController player)
    {
        GameObject sh1 = Instantiate(shotPrefab, shootPos, shotRotation);

        sh1.GetComponent<Rigidbody2D>().AddForce(shotDir * player.shootSpeed, ForceMode2D.Impulse);
        if (player.spriteRenderer.flipX) sh1.GetComponent<SpriteRenderer>().flipX = true;
    }
    private IEnumerator Shoot3Attack(GameObject shootPrefab, Vector2 shootPos, Quaternion shootRotation, Vector2 shootDir, PlayerController player)
    {
        GameObject sh1 = Instantiate(shootPrefab, shootPos, shootRotation);
        GameObject sh2 = Instantiate(shootPrefab, shootPos + new Vector2(0,0.01f), shootRotation);
        sh2.SetActive(false);
        GameObject sh3 = Instantiate(shootPrefab, shootPos - new Vector2(0, 0.01f), shootRotation); 
        sh3.SetActive(false);
        
        sh1.GetComponent<Rigidbody2D>().AddForce(shootDir * player.shootSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        sh2.SetActive(true);
        sh2.GetComponent<Rigidbody2D>().AddForce(shootDir * player.shootSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        sh3.SetActive(true);
        sh3.GetComponent<Rigidbody2D>().AddForce(shootDir * player.shootSpeed, ForceMode2D.Impulse);
        if (player.spriteRenderer.flipX)
        {
            sh1.GetComponent<SpriteRenderer>().flipX = true;
            sh1.GetComponent<SpriteRenderer>().flipX = true;
            sh1.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    private void shotgunAttack(GameObject shotPrefab, Vector2 shootPos, Quaternion shotRotation, Vector2 shotDir, PlayerController player)
    {
        GameObject[] sh = new GameObject[9]; 
        for(int i =0; i<=9; i++)
        {
            float range = ((i / 100) - 0.05f);
            GameObject sh1 = Instantiate(shotPrefab, shootPos + new Vector2(0, range), shotRotation);
            //h[i] = sh1;
            sh1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            sh1.GetComponent<Rigidbody2D>().AddForce(shotDir * player.shootSpeed*0.5f, ForceMode2D.Impulse);
            if (player.spriteRenderer.flipX) sh1.GetComponent<SpriteRenderer>().flipX = true;
        }

    }

    private void grenadeAttack(GameObject shotPrefab, Vector2 shootPos, Quaternion shotRotation, Vector2 shotDir, PlayerController player)
    {
        GameObject sh1 = Instantiate(grenade, shootPos, shotRotation);
        sh1.GetComponent<Rigidbody2D>().gravityScale = 1;
        sh1.GetComponent<Rigidbody2D>().AddForce(shotDir * player.shootSpeed*0.2f, ForceMode2D.Impulse);
        if (player.spriteRenderer.flipX) sh1.GetComponent<SpriteRenderer>().flipX = true;

        Destroy(sh1,3f);
    }
}
