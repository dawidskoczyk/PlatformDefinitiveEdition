using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class RangeAttack : IAttack
{

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
        Vector2 shootDirection = shotDir;
        float angle = Mathf.Atan2(shotDir.y, shotDir.x) * Mathf.Rad2Deg;

        if (shotDir.x < 0)
            player.spriteRenderer.flipX = true;
        else
            player.spriteRenderer.flipX = false;

        Quaternion rotation = player.spriteRenderer.flipX ? Quaternion.Euler(0, 0, angle + 180f) : Quaternion.Euler(0, 0, angle);

        GameObject sh1 = Instantiate(player.shoot1, shotPosition, rotation);
        //Vector2 shootDirection = player.spriteRenderer.flipX ? Vector2.left : Vector2.right;
        
        sh1.GetComponent<Rigidbody2D>().AddForce(shootDirection * player.shootSpeed, ForceMode2D.Impulse);
        if (player.spriteRenderer.flipX) sh1.GetComponent<SpriteRenderer>().flipX = true;

        player.rb.gravityScale = 1;
        player.rb.linearVelocity = savedVelocity;

        player.isLocked = false;

        if (!player.GroundCheck()) player.ChangeCharacterState(PlayerController.PlayerState.Jump);
        else player.ChangeCharacterState(PlayerController.PlayerState.Idle);

        player.canAttack = false;
    }
}
