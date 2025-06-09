using UnityEngine;

public class AttackState : IState
{
    private PlayerControllerSM player;
    public bool lockedState;

    public AttackState(PlayerControllerSM player)
    {
        this.player = player;
    }

    public void Enter()
    {
        lockedState = true;
        player.StartCoroutine(player.gameObject.GetComponentInChildren<IAttack>().Attack(player));
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        if (!lockedState)
        {
            if (!player.IsGrounded())
                player.GetStateMachine().TransitionTo(player.GetStateMachine().jumpState);
            else
                player.GetStateMachine().TransitionTo(player.GetStateMachine().idleState);
        }
            
    }
    public void Exit()
    {
        // code that runs when we exit the state
        player.leftClick = false;
        player.rightClick = false;
    }
}
