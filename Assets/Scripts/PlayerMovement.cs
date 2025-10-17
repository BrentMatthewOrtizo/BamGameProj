using Game.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using Game399.Shared.Diagnostics;


public class PlayerMovement : MonoBehaviour
{
    private IGameLog _logger;
    
    
    public Animator animator;
    
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private float horizontalMovement;
    
    private bool facingRight = true; // Tracks which way the player is facing


    public float jumpPower = 7f;

    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(.5f, .05f);
    public LayerMask groundLayer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _logger = ServiceResolver.Resolve<IGameLog>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        
        // Flip sprite based on movement direction
        if (horizontalMovement > 0 && !facingRight)
            Flip();
        else if (horizontalMovement < 0 && facingRight)
            Flip();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Jump when pressed
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            animator.SetTrigger("jump");
            AudioManager.Instance?.PlayJumpSFX();
            _logger.Info("Player jumped.");
        }

        // Shorten jump when released (only if still moving upward)
        if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private bool IsGrounded()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
    
    private void Flip()
    {
        facingRight = !facingRight; // Toggle direction

        Vector3 scale = transform.localScale;
        scale.x *= -1;              // Multiply X scale by -1 to mirror
        transform.localScale = scale;
    }

}
