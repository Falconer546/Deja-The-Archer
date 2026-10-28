public class e_GroundedState : State<CharacterMotor>
{
    public e_GroundedState(CharacterMotor owner, StateMachine<CharacterMotor> machine) : base(owner, machine) { }

    public override void Tick()
    {
        Owner.RunHorizontal();

        if (!Owner.IsGrounded)
        {
            Machine.ChangeState(Owner.Fall);
            return;
        }

        // jumpCooldown = minimum time on the ground before another jump is allowed
        if (Owner.Intent.JumpHeld && Machine.TimeInState >= Owner.jumpCooldown)
        {
            Machine.ChangeState(Owner.Jump);
            return;
        }

        if (Owner.Intent.DashPressed && Owner.Dash.IsReady)
            Machine.ChangeState(Owner.Dash);
    }
}
