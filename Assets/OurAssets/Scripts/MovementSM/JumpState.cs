using UnityEngine;

public class JumpState
{
    private PlayerControllerSM player;

    public JumpState(PlayerControllerSM player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // code that runs when we first enter the state
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
