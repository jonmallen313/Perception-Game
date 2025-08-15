using Unity.VisualScripting;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isGrounded;

    private PlayerControls controls;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void Awake()
    {
        //
        controls = new PlayerControls();

        


    }

    private void OnEnable()
    {
        //turn on input controls
        controls.Gameplay.Enable();
        //listen for move input
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;

        //listen for jump
        controls.Gameplay.Jump.performed += ctx => jumpPressed = true;

    }

    private void OnDisable()
    {
        controls.Gameplay.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled -= ctx => moveInput = Vector2.zero;
        controls.Gameplay.Jump.performed -= ctx => jumpPressed = true;
        controls.Gameplay.Disable();
        controls.Gameplay.Disable();
    }

    private void FixedUpdate()
    {
        //basic l/r movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        //jump if pressed && grounded
        if (jumpPressed && isGrounded) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        jumpPressed = false;
    }

    private void OnDrawGismosSelected()
    {
        if (groundCheck != null)
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
