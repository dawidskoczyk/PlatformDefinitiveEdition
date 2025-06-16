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
        player.canDash = false;

    }
    public void Update()
    {
        if (!isLocked)
        {
            //UnityEngine.Debug.Log("unlocked switch");
            if (!player.IsGrounded())
            {
                player.GetStateMachine().TransitionTo(player.GetStateMachine().jumpState);
            }
            else if (player.isGettingDmg)
            {
                player.GetStateMachine().TransitionTo(player.GetStateMachine().damageState);
            }
            else
            {
                player.GetStateMachine().TransitionTo(player.GetStateMachine().idleState);
            }
        }


    }
    public void Exit()
    {
        player.GetRigidbody().linearVelocity = Vector2.zero;
        player.GetRigidbody().linearDamping = 5;
        //dash pressed = false -> jest na koncu Dash()
        
    }
}
