/// <summary>
/// Airborne and not rising. Also hosts coyote time: if we walked off a ledge
/// (previous state was e_GroundedState) a jump is still allowed for a short window.
/// </summary>
public class e_FallState : State<CharacterMotor>
{
    private bool _coyoteAvailable;

    public e_FallState(CharacterMotor owner, StateMachine<CharacterMotor> machine) : base(owner, machine) { }

    public override void Enter()
    {
        _coyoteAvailable = Machine.Previous is e_GroundedState;
    }

    public override void Tick()
    {
        Owner.RunHorizontal();
        Owner.AddDownwardForce(Owner.fallForce);

        if (Owner.IsGrounded && Owner.Body.linearVelocity.y <= 0.01f)
        {
            Machine.ChangeState(Owner.Grounded);
            return;
        }

        if (_coyoteAvailable && Owner.Intent.JumpHeld && Machine.TimeInState <= Owner.coyoteTime)
        {
            Machine.ChangeState(Owner.Jump);
            return;
        }

        if (Owner.Intent.DashPressed && Owner.Dash.IsReady)
            Machine.ChangeState(Owner.Dash);
    }
}
