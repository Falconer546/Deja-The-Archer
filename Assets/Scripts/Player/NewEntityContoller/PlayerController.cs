using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player "brain": turns Input System actions into an ICharacterIntent.
/// All movement itself is done by the CharacterMotor on the same GameObject.
/// </summary>
[RequireComponent(typeof(CharacterMotor))]
public class PlayerController : MonoBehaviour, ICharacterIntent
{
    [Tooltip("A dash press stays valid this long, so pressing slightly early still works.")]
    [SerializeField] private float dashBufferTime = 0.1f;

    // DEBUG

    public Bow _bow;

    // DEBUG



    private CharacterMotor _motor;
    private InputAction _moveAction, _jumpAction, _dashAction, _attackAction;
    private float _dashBufferedUntil;

    // --- ICharacterIntent ---
    public Vector2 Move { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool DashPressed => Time.time < _dashBufferedUntil;
    public void ConsumeDash() => _dashBufferedUntil = 0f;

    private void Awake()
    {
        _motor = GetComponent<CharacterMotor>();

        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _dashAction = InputSystem.actions.FindAction("Sprint");
        _attackAction = InputSystem.actions.FindAction("Attack");
    }

    private void Update()
    {
        Move = _moveAction.ReadValue<Vector2>();
        JumpHeld = _jumpAction.IsPressed();

        if (_dashAction.WasPressedThisFrame())
            _dashBufferedUntil = Time.time + dashBufferTime;

        if (_attackAction.WasPressedThisFrame())
            Attack();
    }

    private void Attack() => _bow.ShootArrow(_bow.aimPoint.position - _bow.shootPoint.position);
}
