using UnityEngine;

/// <summary>Run toward the target, hop over walls, stop at ledges. Gives up when the target is too far.</summary>
public class e_ChaseState : State<EnemyController>
{
    public e_ChaseState(EnemyController owner, StateMachine<EnemyController> machine) : base(owner, machine) { }

    public override void Tick()
    {
        if (Owner.Target == null || Owner.DistanceToTarget > Owner.loseRange)
        {
            Machine.ChangeState(Owner.Patrol);
            return;
        }

        CharacterMotor motor = Owner.Motor;

        float dx = Owner.Target.position.x - Owner.transform.position.x;
        float direction = Mathf.Abs(dx) > Owner.stopDistance ? Mathf.Sign(dx) : 0f;

        // Don't run off a platform edge while chasing.
        if (motor.IsGrounded && motor.IsLedgeAhead && direction == motor.FacingDirection)
            direction = 0f;

        Owner.Move = new Vector2(direction * Owner.chaseSpeed, 0f);

        // Hop over obstacles in the way.
        Owner.JumpHeld = Owner.canJumpObstacles && direction != 0f
                         && motor.IsGrounded && motor.IsTouchingWall;
    }

    public override void Exit()
    {
        Owner.Move = Vector2.zero;
        Owner.JumpHeld = false;
    }
}
