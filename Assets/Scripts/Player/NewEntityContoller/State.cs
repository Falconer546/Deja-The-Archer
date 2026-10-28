/// <summary>
/// Base class for every state of every state machine (locomotion, enemy AI, ...).
/// A state owns its transitions: it decides when to hand over to another state
/// via Machine.ChangeState(...). Convention: always `return` right after ChangeState.
/// </summary>
public abstract class State<TOwner>
{
    protected readonly TOwner Owner;
    protected readonly StateMachine<TOwner> Machine;

    protected State(TOwner owner, StateMachine<TOwner> machine)
    {
        Owner = owner;
        Machine = machine;
    }

    public string Name => GetType().Name;

    public virtual void Enter() { }
    public virtual void Exit() { }

    /// <summary>Runs every FixedUpdate: apply behaviour, then check transitions.</summary>
    public virtual void Tick() { }
}
