using System;
using System.Collections.Generic;
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

    public InputAction moveAction, jumpAction, dashAction, attackAction;

    public bool isGrounded, wasGrounded, isFalling, isJumping, canDash, isTouchingWall;
    public bool jumpInit, dashInit, attackInit;

    private Vector2 _velocity = Vector2.one;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PolygonCollider2D groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject bullet;

// DEBUG
    RaycastHit2D wallCheckRay;

    private float _cooldown = 0.5f;
    private float _nextDoorUse = 0f;

    private enum entity_zLayers {entiyLayerF2, entiyLayerF1, entiyLayer0, entiyLaeyrB1, entiyLayerB2};

    entity_zLayers entityActiveLayer = entity_zLayers.entiyLayer0;
    
// DEBUG

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Sprint");
        attackAction = InputSystem.actions.FindAction("Attack");

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
        
        // animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        // animator.SetFloat("yVelocity", rb.linearVelocity.y);
        // animator.SetBool("isGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
        Sprint();
        MoveEntityInLayer();

        FallForce();
        JumpForce();
        WallCheck();

        /*if(!isJumping || isFalling)
        {
            if (!isGrounded)
                rb.AddForce(Vector2.down * 8f, ForceMode2D.Force);
            else
                rb.AddForce(Vector2.down * 40, ForceMode2D.Force);
        }*/
    }

    // %%% Player Movement %%%

    public void GetInputs()
    {
        moveDirection = moveAction.ReadValue<Vector2>();
        jumpInit = jumpAction.IsPressed();
        dashInit = dashAction.IsPressed();
        attackInit = attackAction.IsPressed(); 

    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !isFacingRight;
    }

    private void Move()
    {
        Vector2 _targetVelocity;

        _targetVelocity = new Vector2(moveDirection.x * maxMoveSpeed * 10f, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, _targetVelocity, ref _velocity, 0.1f);
        
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

    private void Atttack()
    {
        if(attackInit)
        {
            Instantiate(bullet);
        }
    }

    private void Sprint()
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
        canDash = true;
    }

    private void WallCheck()
    {
        int _direction = isFacingRight ? 1 : -1;
        wallCheckRay = Physics2D.Raycast(transform.position, Vector2.right * _direction * 2);
        isTouchingWall = wallCheckRay.collider != null;
    }

    public void MoveEntityInLayer()
    {
        if (DoorMechanics.useableDoor != null && moveDirection.y > 0 && Time.time >= _nextDoorUse)
        {
            Transform door = DoorMechanics.useableDoor.transform;
            Vector3 playerPos = transform.position;

            float entityToDoorDistance = door.position.z - transform.position.z;
            Debug.Log($"EntityDoor_Distance: {entityToDoorDistance}");

            if (entityToDoorDistance < 0 && Math.Abs(entityToDoorDistance) < 0.16f)
            {
                playerPos = new Vector3(
                    playerPos.x,//Mathf.Lerp(playerPos.x, door.position.x, 0.5f), //use lerp???
                    playerPos.y,//Mathf.Lerp(playerPos.y, door.position.y, 0.5f),
                    playerPos.z - 0.2f
                );
                ChangeSortingLayer(spriteRenderer.sortingLayerName, + 2);
                entityActiveLayer -= 1;
            }
            else if (entityToDoorDistance > 0 && Math.Abs(entityToDoorDistance) < 0.16f)
            {
                playerPos = new Vector3(
                    playerPos.x,//Mathf.Lerp(playerPos.x, door.position.x, 0.5f),
                    playerPos.y,//Mathf.Lerp(playerPos.y, door.position.y, 0.5f),
                    playerPos.z + 0.2f
                );
                ChangeSortingLayer(spriteRenderer.sortingLayerName, - 2);
                entityActiveLayer += 1;
            }

            transform.position = playerPos;
            _nextDoorUse = Time.time + _cooldown;
        }
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

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; // volitelné, aby to fungovalo i v Editoru mimo běh

        Vector2 origin = transform.position;
        Vector2 direction = Vector2.right * moveDirection.x;

        Gizmos.color = isTouchingWall ? Color.green : Color.red;
        Vector2 endPoint = isTouchingWall ? wallCheckRay.point : origin + direction;
        Gizmos.DrawLine(origin, endPoint);

        // Malá kulička na místě dopadu (pokud něco trefilo)
        if (isTouchingWall)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(wallCheckRay.point, 0.1f);
        }
    }

    //##############################################################################################################

    private void ChangeSortingLayer(string activeLayerName, int change)
    {
        List<SortingLayer> sortingLayers = new List<SortingLayer>();
        sortingLayers.AddRange(SortingLayer.layers);

        int layerValue = 0;
        foreach (SortingLayer layer in sortingLayers)
        {
            if (activeLayerName == layer.name)
            {
                layerValue = layer.value;
            }
        }

        spriteRenderer.sortingLayerName = sortingLayers[layerValue + change].name;
    } 
}