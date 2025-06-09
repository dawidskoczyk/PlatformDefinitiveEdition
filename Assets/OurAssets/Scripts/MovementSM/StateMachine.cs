using System;
using Unity.IO.LowLevel.Unsafe;

[Serializable]
public class StateMachine
{
    public IState CurrentState { get; private set; }
    public RunState runState;
    public JumpState jumpState;
    public IdleState idleState;
    public AttackState attackState;

    public StateMachine(PlayerControllerSM player)
    {
        this.runState = new RunState(player);
        this.jumpState = new JumpState(player);
        this.idleState = new IdleState(player);
        this.attackState = new AttackState(player);  
    }

    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }
    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
    }
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}
