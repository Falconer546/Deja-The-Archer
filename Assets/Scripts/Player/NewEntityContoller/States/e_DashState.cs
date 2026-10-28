using UnityEngine;

/// <summary>
/// Fixed-duration horizontal dash with gravity suspended. The cooldown starts
/// when the dash ends; other states ask Dash.IsReady before entering.
/// </summary>
public class e_DashState : State<CharacterMotor>
{
    private float _nextAvailableTime;
    private float _direction;
    private float _savedGravityScale;

    public e_DashState(CharacterMotor owner, StateMachine<CharacterMotor> machine) : base(owner, machine) { }

    public bool IsReady => Owner.canDash && Time.time >= _nextAvailableTime;

    public override void Enter()
    {
        Rigidbody2D rb = Owner.Body;

        Owner.Intent.ConsumeDash();

        _direction = Owner.FacingDirection;
        _savedGravityScale = rb.gravityScale;

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(_direction * Owner.dashSpeed, 0f);
    }

    public override void Tick()
    {
        // Re-assert every step so friction/drag can't shorten the dash.
        Owner.Body.linearVelocity = new Vector2(_direction * Owner.dashSpeed, 0f);

        if (Machine.TimeInState >= Owner.dashDuration)
            Machine.ChangeState(Owner.IsGrounded ? (State<CharacterMotor>)Owner.Grounded : Owner.Fall);
    }

    public override void Exit()
    {
        Owner.Body.gravityScale = _savedGravityScale;
        _nextAvailableTime = Time.time + Owner.dashCooldown;

        // Run smoothing then eases the leftover dash speed down to the run speed.
        Owner.ResetRunSmoothing();
    }
}
