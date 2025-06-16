using System.Threading;
using UnityEngine;

public class JumpState : IState
{
    private PlayerControllerSM player;
    bool isLocked = true;
    float timer = 0;

    public JumpState(PlayerControllerSM player)
    {
        this.player = player;
    }

    public void Enter()
    {

        
    }
    public void Update()
    {
        timer += Time.deltaTime;

        if(timer > 0.3f) // opoznienie, bo inaczej od razu po wyskoku gracz jest jeszcze grounded i przez to zmienia stan zanim wyskoczy
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
            else if (player.isGettingDmg)
            {
                player.GetStateMachine().TransitionTo(player.GetStateMachine().damageState);
            }
        }

        player.Jump();
    }
    public void Exit()
    {
        // code that runs when we exit the state
        player.GetRigidbody().linearDamping = 5;
        player.GetRigidbody().linearVelocityX = 0;
        timer = 0;
    }

    void UnlockStateDetaction()
    {
        isLocked = false;
    }
}
