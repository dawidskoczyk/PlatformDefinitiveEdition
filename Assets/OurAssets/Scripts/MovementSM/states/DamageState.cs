
using System.Threading;
using UnityEngine;

public class DamageState : IState
{
    private PlayerControllerSM player;
    float timer = 0;

    public DamageState(PlayerControllerSM player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.GetRigidbody().linearVelocity = Vector2.zero;
        player.GetRigidbody().AddForce(player.GetPushForce(), ForceMode2D.Impulse);

    }
    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= player.GetStunTime())
        {

            if (player.GetHorizontalInput() != 0 && player.IsGrounded())
            {
                player.GetStateMachine().TransitionTo(player.GetStateMachine().runState);
            }
            else if (player.GetHorizontalInput() == 0 && player.IsGrounded())
            {
                player.GetStateMachine().TransitionTo(player.GetStateMachine().idleState);
            }
            else if (player.leftClick || player.rightClick)
            {
                player.GetStateMachine().TransitionTo(player.GetStateMachine().attackState);
            }
            else if (player.IsDashPressed() && player.canDash)
            {
                player.GetStateMachine().TransitionTo(player.GetStateMachine().dashState);
            }
            else if (!player.IsGrounded())
            {
                player.GetStateMachine().TransitionTo(player.GetStateMachine().jumpState);
            }
        }


    
    }
    public void Exit()
    {
        // code that runs when we exit the state
        timer = 0;
        player.isGettingDmg = false;
    }
}
