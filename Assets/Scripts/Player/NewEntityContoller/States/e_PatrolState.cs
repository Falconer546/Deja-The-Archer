using UnityEngine;

/// <summary>Walk back and forth; turn around at walls and ledges; chase when the target is close.</summary>
public class e_PatrolState : State<EnemyController>
{
    private float _direction = 1f;
    private float _nextTurnTime;

    public e_PatrolState(EnemyController owner, StateMachine<EnemyController> machine) : base(owner, machine) { }

    public override void Enter()
    {
        _direction = Owner.Motor.FacingDirection;
        Owner.JumpHeld = false;
    }

    public override void Tick()
    {
        if (Owner.DistanceToTarget <= Owner.detectionRange)
        {
            Machine.ChangeState(Owner.Chase);
            return;
        }

        CharacterMotor motor = Owner.Motor;
        bool blocked = motor.IsTouchingWall || (motor.IsGrounded && motor.IsLedgeAhead);

        if (blocked && Time.time >= _nextTurnTime)
        {
            _direction = -_direction;
            _nextTurnTime = Time.time + Owner.turnCooldown;
        }

        Owner.Move = new Vector2(_direction * Owner.patrolSpeed, 0f);
    }

    public override void Exit()
    {
        Owner.Move = Vector2.zero;
    }
}
