using UnityEngine;

public class AttackState : IState
{
    private PlayerControllerSM player;

    public AttackState(PlayerControllerSM player)
    {
        this.player = player;
    }

    public void Enter()
    {
        
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }
}
