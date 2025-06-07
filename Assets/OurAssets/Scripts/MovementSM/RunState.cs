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
        Debug.Log("stan biegany");
        player.GetRigidbody().linearDamping = 5;
        player.GetAnimator().Play("Run_Animation");
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        if(player.GetHorizontalInput() == 0)
            player.GetStateMachine().TransitionTo(player.GetStateMachine().idleState);


        player.Move(player.moveSpeed * player.GetHorizontalInput());
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }
}
