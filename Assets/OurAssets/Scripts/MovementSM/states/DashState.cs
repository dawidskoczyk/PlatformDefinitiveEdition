using UnityEngine;

public class DashState : IState
{
    private PlayerControllerSM player;
    public bool isLocked;

    public DashState(PlayerControllerSM player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // code that runs when we first enter the state
        isLocked = true;
        player.Dash();
    }
    public void Update()
    {
        if (!isLocked)
        {
            //UnityEngine.Debug.Log("unlocked switch");
            if (!player.IsGrounded())
                player.GetStateMachine().TransitionTo(player.GetStateMachine().jumpState);
            else
                player.GetStateMachine().TransitionTo(player.GetStateMachine().idleState);
        }


    }
    public void Exit()
    {
        // code that runs when we exit the state
    }
}
