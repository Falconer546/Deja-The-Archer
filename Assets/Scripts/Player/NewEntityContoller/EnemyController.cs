using UnityEngine;

/// <summary>
/// Enemy "brain". Runs its own AI state machine (Patrol / Chase) whose only output
/// is an ICharacterIntent. The shared CharacterMotor turns that intent into movement,
/// so enemies walk, jump and fall exactly like the player.
/// </summary>
[DefaultExecutionOrder(-50)] // run before the motor so it sees this step's intent
[RequireComponent(typeof(CharacterMotor))]
public class EnemyController : MonoBehaviour, ICharacterIntent
{
    [Header("Perception")]
    public float detectionRange = 8f;
    public float loseRange = 12f;
    public float stopDistance = 1f;

    [Header("Behaviour")]
    [Range(0f, 1f)] public float patrolSpeed = 0.5f;   // fraction of the motor's max speed
    [Range(0f, 1f)] public float chaseSpeed = 1f;
    public bool canJumpObstacles = true;
    public float turnCooldown = 0.3f;

    [Header("Target (auto-found if empty)")]
    [SerializeField] private Transform target;

    [Header("Debug (read-only)")]
    [SerializeField] private string currentState;

    public CharacterMotor Motor { get; private set; }
    public Transform Target => target;
    public StateMachine<EnemyController> AI { get; private set; }
    public e_PatrolState Patrol { get; private set; }
    public e_ChaseState Chase { get; private set; }

    /// <summary>Distance to the target, or infinity when there is none.</summary>
    public float DistanceToTarget =>
        target == null ? float.PositiveInfinity : Vector2.Distance(transform.position, target.position);

    // --- ICharacterIntent: written by the AI states ---
    public Vector2 Move { get; set; }
    public bool JumpHeld { get; set; }
    public bool DashPressed => _dashRequested;
    public void RequestDash() => _dashRequested = true;
    public void ConsumeDash() => _dashRequested = false;

    private bool _dashRequested;

    private void Awake()
    {
        Motor = GetComponent<CharacterMotor>();

        AI = new StateMachine<EnemyController>();
        Patrol = new e_PatrolState(this, AI);
        Chase = new e_ChaseState(this, AI);
    }

    private void Start()
    {
        if (target == null)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null) target = player.transform;
        }

        AI.Initialize(Patrol);
    }

    private void FixedUpdate()
    {
        AI.Tick();
        currentState = AI.Current.Name;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}
