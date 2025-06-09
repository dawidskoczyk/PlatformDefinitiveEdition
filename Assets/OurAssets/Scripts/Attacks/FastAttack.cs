using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
public class FastAttack : IAttack
{
    [SerializeField] float animatorSpeed = 1;
    [SerializeField] float waitSpeed = 0.2f;
    [SerializeField] float shootSpeed = 100;
    [SerializeField] GameObject projectile;

    //private void Start()
    //{
        
    //}
    public override IEnumerator Attack(PlayerControllerSM player)
    {
        // player.isLocked = true;
        
        if (!Input.GetKey(KeyCode.W))
            player.upAttack = false;//nie powinno tu byæ inputów ale póki dzia³a to niech bêd¹
        if (!Input.GetKey(KeyCode.S))
            player.slam = false;

        if (player.upAttack)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(0, 1), new Vector2(1, 1), 0);

            if (hitCollider != null)
            {
                if (hitCollider.transform.tag == "Enemy")
                    hitCollider.GetComponent<Hearts>().SubHp(1);
            }

            if (!Input.GetKey(KeyCode.W))
                player.upAttack = false;
           
            if (CombatManager.Perks["CritDmg"])

                player.GetAnimator().speed = 0.75f;
            else
                player.GetAnimator().speed = animatorSpeed;

            player.GetAnimator().Play("AirSlashUp");

            if (CombatManager.Perks["rangeSlash"])
            {
                print("wchodzi do rangeSlash "+ CombatManager.Perks["rangeSlash"]);
                spawnProjectile(player, new Vector3(0, 1)); //fastAttackHardPerk //CritDmg
            }
            if (CombatManager.Perks["CritDmg"])
            {
                player.GetAnimator().speed = 0.75f;
            }

        }
        else if (player.slam)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(0, -1), new Vector2(1, 1), 0);

            if (hitCollider != null)
            {
                if (hitCollider.transform.tag == "Enemy")
                    hitCollider.GetComponent<Hearts>().SubHp(1);
            }

            if (!Input.GetKey(KeyCode.S))
                player.slam = false;

            if (CombatManager.Perks["CritDmg"])
                player.GetAnimator().speed = 0.75f;
            else
                player.GetAnimator().speed = animatorSpeed;
            player.GetAnimator().Play("AirSlashDown");

            if (CombatManager.Perks["rangeSlash"])
            {
                spawnProjectile(player, new Vector3(0, -1));
            }

        }
        else if (player.spriteRenderer.flipX)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(-1, 0), new Vector2(1, 1), 0);

            if (hitCollider != null)
            {
                if (hitCollider.transform.tag == "Enemy")
                    hitCollider.GetComponent<Hearts>().SubHp(1);
            }

            player.GetAnimator().Play("fastSlash");

            if (CombatManager.Perks["rangeSlash"])
            {
                spawnProjectile(player, new Vector3(-1, 0));
            }

            if (CombatManager.Perks["CritDmg"])
            {
                player.GetAnimator().Play("fastAttackHardPerk");
            }
        }
        else
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + new Vector3(1, 0), new Vector2(1, 1), 0);

            if (hitCollider != null)
            {
                if (hitCollider.transform.tag == "Enemy")
                    hitCollider.GetComponent<Hearts>().SubHp(1);
            }

            player.GetAnimator().Play("fastSlash");

            if (CombatManager.Perks["rangeSlash"])
            {
                spawnProjectile(player, new Vector3(1, 0));
            }

            if (CombatManager.Perks["CritDmg"])
            {
                player.GetAnimator().Play("fastAttackHardPerk");
            }
        }
        if (CombatManager.Perks["CritDmg"])
            waitSpeed = 0.75f;
        else
            waitSpeed = 0.2f;

        yield return new WaitForSeconds(waitSpeed);

        player.GetAnimator().speed = 1;

        player.GetStateMachine().attackState.lockedState = false;
    }
    private void spawnProjectile(PlayerControllerSM player, Vector2 direction)
    {
        Vector2 shotPosition = player.spriteRenderer.flipX ? transform.TransformPoint(new Vector2(-player.gunSpot.localPosition.x, player.gunSpot.localPosition.y)) : player.gunSpot.position;
        GameObject GO = Instantiate(projectile, shotPosition, Quaternion.identity);
        Vector2 shootDirection = direction;
        
        GO.GetComponent<Rigidbody2D>().AddForce(shootDirection * shootSpeed, ForceMode2D.Impulse);
        if (player.spriteRenderer.flipX) GO.GetComponent<SpriteRenderer>().flipX = true;
        Destroy(GO, 0.3f);
    }

}
