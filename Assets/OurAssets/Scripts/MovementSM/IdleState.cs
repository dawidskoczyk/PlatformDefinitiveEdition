using System.Diagnostics;
using Unity.VisualScripting.FullSerializer;

using System;
using UnityEngine;
public class IdleState : IState
{
    private PlayerControllerSM player;

    public IdleState(PlayerControllerSM player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // code that runs when we first enter the state
        UnityEngine.Debug.Log("idle state on");
        //rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
       
        player.GetRigidbody().linearDamping = 5;
        player.GetRigidbody().gravityScale = 1;
        player.GetAnimator().Play("idle-Animation");

    }
    public void Update()
    {
 // Here we add logic to detect if the conditions exist to
 // transition to another state
        if(player.GetHorizontalInput() != 0)
        {
            player.GetStateMachine().TransitionTo(player.GetStateMachine().walkState);
        }

    }
    public void Exit()
    {
        // code that runs when we exit the state
    }
}

// DashState