using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public Vector2 moveDirection, moveVelocity;
    public float jumpForce = 18f;
    public float jumpDumping = 80f;
    public float fallForce = 35f;
    public float jumpCooldown = 0.12f;
    public float groundedTimer = 0f;
    public float coyoteTime = 0.12f;

    public float maxMoveSpeed = 1f;
    public float smoothMoveSpeed = 0.1f;
    public float dashForce = 5f;
    public bool isFacingRight = true;

    public InputAction moveAction, jumpAction, dashAction;

    public bool isGrounded, wasGrounded, isFalling, isJumping, canDash;
    public bool jumpInit, dashInit;

    private Vector2 _velocity = Vector2.zero;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PolygonCollider2D groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

// DEBUG

// DEBUG

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Sprint");

        groundCheck = GetComponentInChildren<PolygonCollider2D>();
        groundLayer = LayerMask.GetMask("Ground");
    } 

    void Update()
    {
        GetInputs();

        if(isFacingRight && moveDirection.x < 0f || !isFacingRight && moveDirection.x > 0f)
            Flip();

        GroundCheck();
        GroundedTimer();
        FallingCheck();
        JumpingCheck();
        DashCheck();



        wasGrounded = isGrounded;

        moveVelocity = rb.linearVelocity;
        
        animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
        Dash();

        FallForce();
        JumpForce();
    }

    // %%% Player Movement %%%

    public void GetInputs()
    {
        moveDirection = moveAction.ReadValue<Vector2>();
        jumpInit = jumpAction.IsPressed();
        dashInit = dashAction.IsPressed();

    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !isFacingRight;
    }

    private void Move()
    {
        Vector2 targetVelocity;
        Vector2 smoothVelocity;
        float scaledMoveX;

        if (isGrounded)
        {
            scaledMoveX = moveDirection.x * 10f;
            targetVelocity = new Vector2(scaledMoveX * maxMoveSpeed, 0);
        }
        else
        {
            scaledMoveX = moveDirection.x * Mathf.Clamp(Mathf.Abs(groundedTimer + 0.5f), 0.5f, 1f) * 9f;
            targetVelocity = new Vector2(rb.linearVelocity.x * 0.55f + scaledMoveX, rb.linearVelocity.y);
        }

        smoothVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref _velocity, smoothMoveSpeed);
        rb.linearVelocity = smoothVelocity;
    }
    private void Jump()
    {
        if(jumpInit)
        {
            if(isGrounded && groundedTimer > jumpCooldown || groundedTimer < 0 && groundedTimer > -coyoteTime)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                rb.AddForce(Vector2.down * jumpDumping, ForceMode2D.Force);
            }
        }
    }

    private void Dash()
    {    
        if(dashInit && canDash)
        {
            if(isFacingRight)
            { 
                Debug.Log("Dash R");
                rb.AddForce(dashForce * Vector2.right, ForceMode2D.Impulse);
            }
            if(!isFacingRight)
            {
                Debug.Log("Dash L");
                rb.AddForce(dashForce * Vector2.left, ForceMode2D.Impulse);
            }
        }
    }

    private void FallForce()
    {
        if(isFalling)
            rb.AddForce(Vector2.down * fallForce, ForceMode2D.Force);
    }

    private void JumpForce()
    {
        if(isJumping)
            rb.AddForce(Vector2.down * jumpDumping, ForceMode2D.Force);
    }

    // %%% State Check %%%

    private void GroundCheck()
    {
        isGrounded = groundCheck.IsTouchingLayers(groundLayer);
    }

    private void FallingCheck()
    {
        isFalling = rb.linearVelocity.y < -0.5f && groundedTimer < 0f;
    }

    private void JumpingCheck()
    {
        isJumping = rb.linearVelocity.y > 0.5f && groundedTimer < 0f;
    }

    private void DashCheck()
    {
        canDash = isGrounded;
    }


    private void GroundedTimer()
    {
        if(isGrounded && wasGrounded == true)
            groundedTimer += 1 * Time.deltaTime;
        else if(!isGrounded && !wasGrounded == true)
            groundedTimer -= 1 * Time.deltaTime;
        else
            groundedTimer = 0f;
    }
}
