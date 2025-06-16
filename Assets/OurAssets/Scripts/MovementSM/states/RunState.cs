using UnityEngine;

public class RunState : IState
{
    private PlayerControllerSM player;

    public RunState(PlayerControllerSM player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // code that runs when we first enter the state
        player.GetRigidbody().linearDamping = 5;
        player.GetAnimator().Play("Run_Animation");

        player.jumpCounter = 0;
        player.canDash = true;
        player.IsGrounded();

    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        player.Move(player.moveSpeed * player.GetHorizontalInput());

        if (player.GetHorizontalInput() == 0 && player.IsGrounded())
        {
            player.GetStateMachine().TransitionTo(player.GetStateMachine().idleState);
        }
        else if (player.IsJumpPressed() || (player.GetRigidbody().linearVelocityY < -1 && !player.IsGrounded()))
        {
            player.GetStateMachine().TransitionTo(player.GetStateMachine().jumpState);
        }
        else if (player.leftClick || player.rightClick)
        {
            player.GetStateMachine().TransitionTo(player.GetStateMachine().attackState);
        }
        else if (player.IsDashPressed() && player.canDash)
        {
            player.GetStateMachine().TransitionTo(player.GetStateMachine().dashState);
        }
        else if (player.isGettingDmg)
        {
            player.GetStateMachine().TransitionTo(player.GetStateMachine().damageState);
        }


    }
    public void Exit()
    {
        // code that runs when we exit the state
        
    }
}
