using UnityEngine;

/// <summary>
/// What a character WANTS to do this physics step. The CharacterMotor reads it and
/// turns it into movement. A "brain" (PlayerController, EnemyController, a cutscene
/// script, ...) implements this and decides where the intent comes from.
/// </summary>
public interface ICharacterIntent
{
    /// <summary>Desired movement, -1..1 per axis (x = run, y = up for doors/ladders).</summary>
    Vector2 Move { get; }

    /// <summary>Jump is wanted while true.</summary>
    bool JumpHeld { get; }

    /// <summary>A dash was requested and has not been consumed yet.</summary>
    bool DashPressed { get; }

    /// <summary>Called by the motor when it actually starts the dash.</summary>
    void ConsumeDash();
}
