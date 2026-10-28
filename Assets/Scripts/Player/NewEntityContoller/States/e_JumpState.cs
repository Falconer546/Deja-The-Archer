using UnityEngine;

/// <summary>Rising part of a jump. Ends at the apex, when vertical velocity reaches zero.</summary>
public class e_JumpState : State<CharacterMotor>
{
    public e_JumpState(CharacterMotor owner, StateMachine<CharacterMotor> machine) : base(owner, machine) { }

    public override void Enter()
    {
        Rigidbody2D rb = Owner.Body;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * Owner.jumpForce, ForceMode2D.Impulse);
    }

    public override void Tick()
    {
        Owner.RunHorizontal();
        Owner.AddDownwardForce(Owner.jumpDumping);

        if (Owner.Intent.DashPressed && Owner.Dash.IsReady)
        {
            Machine.ChangeState(Owner.Dash);
            return;
        }

        if (Owner.Body.linearVelocity.y <= 0f)
            Machine.ChangeState(Owner.Fall);
    }
}
