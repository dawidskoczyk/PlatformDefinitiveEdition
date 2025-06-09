//using System.Collections;
//using UnityEngine;

//public class MeleeAttack : IAttack
//{
//    public override IEnumerator Attack(PlayerControllerSM player)
//    {
//        player.isLocked = true;

//        if (player.upAttack)
//        {
//            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(0, 1), new Vector2(1, 1), 0);
//            Debug.Log("up hit : " + hitCollider);
//            player.upAttack = false;
//            player.animator.Play("AirSlashUp");
//        }
//        else if (player.slam)
//        {
//            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(0, -1), new Vector2(1, 1), 0);
//            Debug.Log("down hit : " + hitCollider);
//            player.slam = false;
//            player.animator.Play("AirSlashDown");
//        }
//        else if (player.spriteRenderer.flipX)
//        {
//            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(-1, 0), new Vector2(1, 1), 0);
//            Debug.Log("left hit : " + hitCollider);
//            player.animator.Play("SwordCombo1");
//        }
//        else
//        {
//            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(1, 0), new Vector2(1, 1), 0);
//            Debug.Log("right hit : " + hitCollider);
//            player.animator.Play("SwordCombo1");
//        }

//        yield return new WaitForSeconds(0.5f);
//        player.isLocked = false;

//        if (!player.GroundCheck()) player.ChangeCharacterState(PlayerController.PlayerState.Jump);
//        else player.ChangeCharacterState(PlayerController.PlayerState.Idle);

//        player.canAttack = false;
//    }
//}
