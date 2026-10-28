using UnityEngine;

/// <summary>
/// Shared platformer body for ANY character (player, enemy, NPC).
/// Reads an ICharacterIntent from a brain component on the same GameObject,
/// runs ground / wall / ledge sensors and drives the locomotion state machine
/// (e_GroundedState, e_JumpState, e_FallState, e_DashState).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMotor : MonoBehaviour
{
    [Header("Run")]
    public float maxMoveSpeed = 1f;
    public float speedScale = 10f;          // target speed = Move.x * maxMoveSpeed * speedScale
    public float smoothMoveSpeed = 0.1f;

    [Header("Jump")]
    public float jumpForce = 20f;
    public float jumpDumping = 60f;         // extra downward force while rising
    public float jumpCooldown = 0.12f;      // min time on ground before next jump
    public float coyoteTime = 0.12f;

    [Header("Fall")]
    public float fallForce = 35f;           // eile fallingxtra downward force wh

    [Header("Dash")]
    public bool canDash = true;
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.4f;       // counted from the end of the dash

    [Header("Sensors")]
    public float wallCheckDistance = 0.6f;  // ray from the body center, in facing direction
    public float ledgeCheckForward = 0.5f;  // how far in front of the body the ledge ray starts
    public float ledgeCheckDepth = 1.5f;    // how far down the ledge ray looks

    [Header("References (auto-filled if empty)")]
    [SerializeField] private Rigidbody2D rb;
    [Tooltip("Trigger/collider at the feet. Assign explicitly if the body itself has a PolygonCollider2D.")]
    [SerializeField] private Collider2D groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Debug (read-only)")]
    [SerializeField] private string currentState;

    // --- Read by states and other components ---
    public Rigidbody2D Body => rb;
    public ICharacterIntent Intent { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsTouchingWall { get; private set; }
    /// <summary>True when there is no ground in front of the feet (edge of a platform).</summary>
    public bool IsLedgeAhead { get; private set; }
    public bool IsFacingRight { get; private set; } = true;
    public float FacingDirection => IsFacingRight ? 1f : -1f;

    // --- Locomotion state machine ---
    public StateMachine<CharacterMotor> StateMachine { get; private set; }
    public e_GroundedState Grounded { get; private set; }
    public e_JumpState Jump { get; private set; }
    public e_FallState Fall { get; private set; }
    public e_DashState Dash { get; private set; }

    private float _runSmoothingVelocity;
    private RaycastHit2D _wallHit;
    private RaycastHit2D _ledgeHit;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (groundCheck == null) groundCheck = GetComponentInChildren<PolygonCollider2D>();
        if (groundLayer.value == 0) groundLayer = LayerMask.GetMask("Ground");

        StateMachine = new StateMachine<CharacterMotor>();
        Grounded = new e_GroundedState(this, StateMachine);
        Jump = new e_JumpState(this, StateMachine);
        Fall = new e_FallState(this, StateMachine);
        Dash = new e_DashState(this, StateMachine);

        Intent = GetComponent<ICharacterIntent>();
        if (Intent == null)
        {
            Debug.LogError($"{name}: CharacterMotor needs a brain component implementing ICharacterIntent " +
                           "(e.g. PlayerController or EnemyController).", this);
            enabled = false;
        }
    }

    private void Start()
    {
        // Start in Fall; the first tick moves us to Grounded if we are on the floor.
        StateMachine.Initialize(Fall);
    }

    private void Update()
    {
        HandleFacing();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        IsGrounded = groundCheck != null && groundCheck.IsTouchingLayers(groundLayer);
        UpdateSensors();

        StateMachine.Tick();
        currentState = StateMachine.Current.Name;
    }

    // %%% Facing %%%

    private void HandleFacing()
    {
        float x = Intent.Move.x;
        if (IsFacingRight && x < -0.01f || !IsFacingRight && x > 0.01f)
        {
            IsFacingRight = !IsFacingRight;
            if (spriteRenderer != null)
                spriteRenderer.flipX = !IsFacingRight;
        }
    }

    // %%% Helpers used by states %%%

    /// <summary>Smoothly accelerate horizontal velocity toward the intent; vertical is untouched.</summary>
    public void RunHorizontal()
    {
        float targetX = Intent.Move.x * maxMoveSpeed * speedScale;
        float x = Mathf.SmoothDamp(rb.linearVelocity.x, targetX, ref _runSmoothingVelocity, smoothMoveSpeed);
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }

    public void ResetRunSmoothing() => _runSmoothingVelocity = 0f;

    public void AddDownwardForce(float force) =>
        rb.AddForce(Vector2.down * force, ForceMode2D.Force);

    // %%% Sensors %%%

    private void UpdateSensors()
    {
        Vector2 origin = rb.position;
        Vector2 forward = Vector2.right * FacingDirection;

        _wallHit = Physics2D.Raycast(origin, forward, wallCheckDistance, groundLayer);
        IsTouchingWall = _wallHit.collider != null;

        Vector2 ledgeOrigin = origin + forward * ledgeCheckForward;
        _ledgeHit = Physics2D.Raycast(ledgeOrigin, Vector2.down, ledgeCheckDepth, groundLayer);
        IsLedgeAhead = _ledgeHit.collider == null;
    }

    private void UpdateAnimator()
    {
        // TODO: enable once the Animator parameters exist. Or subscribe to
        // StateMachine.StateChanged and drive transitions per state instead.
        // animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        // animator.SetFloat("yVelocity", rb.linearVelocity.y);
        // animator.SetBool("isGrounded", IsGrounded);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Vector2 origin = rb.position;
        Vector2 forward = Vector2.right * FacingDirection;

        // Wall ray
        Gizmos.color = IsTouchingWall ? Color.green : Color.red;
        Gizmos.DrawLine(origin, IsTouchingWall ? _wallHit.point : origin + forward * wallCheckDistance);
        if (IsTouchingWall)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_wallHit.point, 0.1f);
        }

        // Ledge ray (green = ground ahead, red = ledge)
        Vector2 ledgeOrigin = origin + forward * ledgeCheckForward;
        Gizmos.color = IsLedgeAhead ? Color.red : Color.green;
        Gizmos.DrawLine(ledgeOrigin, ledgeOrigin + Vector2.down * ledgeCheckDepth);
    }
}
