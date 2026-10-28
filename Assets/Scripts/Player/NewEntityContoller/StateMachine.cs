using System;
using UnityEngine;

/// <summary>
/// Generic state machine. It knows nothing about players or enemies; it only
/// tracks the current state, the previous one and the time spent in the current one.
/// </summary>
public class StateMachine<TOwner>
{
    public State<TOwner> Current { get; private set; }
    public State<TOwner> Previous { get; private set; }
    public float TimeInState { get; private set; }

    /// <summary>Fired after a transition: (from, to). Handy for animation or audio.</summary>
    public event Action<State<TOwner>, State<TOwner>> StateChanged;

    public void Initialize(State<TOwner> startState)
    {
        Current = startState;
        TimeInState = 0f;
        Current.Enter();
        StateChanged?.Invoke(null, Current);
    }

    public void ChangeState(State<TOwner> next)
    {
        if (next == null || next == Current) return;

        State<TOwner> from = Current;
        from?.Exit();

        Previous = from;
        Current = next;
        TimeInState = 0f;

        Current.Enter();
        StateChanged?.Invoke(from, Current);
    }

    /// <summary>Call once per FixedUpdate.</summary>
    public void Tick()
    {
        TimeInState += Time.fixedDeltaTime;
        Current?.Tick();
    }
}
