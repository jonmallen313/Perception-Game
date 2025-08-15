using System;
using UnityEngine;
using UnityEngine.Serialization;


public class PlayerMovementAlt : MonoBehaviour
{
    private Rigidbody rb;
    private Stats stats;
    private PlayerCombat combat;
    private ShiftConnection sc;
    private Animator anim;
    private AudioSource audioSource;

    public AudioClip WalkingClip;

    private GameObject LastObject;

    private bool grounded;

    // JUMP
    private bool jump;
    public KeyCode JumpKeyCode;


    // SPRINT
    private bool sprint;
    public KeyCode SprintKeyCode;

    // DASH
    private bool dash;
    public KeyCode DashKeyCode;

    private Timer dashTimer;
    public ForceMode DashForceMode;

    // SLAM
    private KeyCode slamKeyCode;
    private bool slam;

    public bool IsSlamming;
    public ForceMode SlamForceMode;

    // MULTI JUMP
    public int MaxJumps;
    public int Jumps;

    public float JumpForce;
    public ForceMode JumpForceMode;

    [Range(-1, 1)] public int DirectionX;
    public bool DirectionLock;

    private PerspectiveShift perspective;

    private float currentSpeed;

    private Timer rangedAnimDurationTimer;
    public float RangedAnimDuration;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<Stats>();
        combat = GetComponent<PlayerCombat>();
        sc = GetComponent<ShiftConnection>();
        anim = GetComponentInChildren<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        rangedAnimDurationTimer = gameObject.AddComponent<Timer>();

        audioSource.clip = WalkingClip;
        audioSource.playOnAwake = false;

        slamKeyCode = combat.SlamAttack.AttackKey;

        dashTimer = gameObject.AddComponent<Timer>();

        perspective = FindFirstObjectByType<PerspectiveShift>();

        Jumps = MaxJumps;

        DirectionX = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // BLOCK ALL MOVEMENT IF DIALOGUE CONVO ACTIVE
        if (ConversationManager.IsDialogueActive)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }


        jump = Input.GetKeyDown(JumpKeyCode);
        sprint = Input.GetKey(SprintKeyCode);
        dash = Input.GetKeyDown(DashKeyCode);
        slam = (Input.GetKeyDown(slamKeyCode) && !grounded);

        // negate x asis movement if in 2D flipped view
        float moveX = Input.GetAxisRaw("Horizontal");
        if (perspective.flipped2DView)
        {
            moveX *= -1;
        }

        if (!DirectionLock && Time.timeScale != 0)
        {
            if (moveX > 0)
            {
                SetDirectionRight();
            }

            if (moveX < 0)
            {
                SetDirectionLeft();
            }
        }

        float moveZ = 0f;

        if (perspective.IsIn3DMode() == true)
        {
            moveZ = Input.GetAxisRaw("Vertical");
        }

        if (perspective.flipped2DView)
        {
            moveZ *= -1;
        }

        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 newVelocity = new Vector3(
            moveX * currentSpeed,
            currentVelocity.y,
            moveZ * currentSpeed
        );

        if (combat.RangedAttack.IsAttacking)
        { 
            rangedAnimDurationTimer.StartTimer(RangedAnimDuration);
        }
        
        if (rangedAnimDurationTimer.IsRunning)
        {
            anim.Play("Shooting");
            anim.speed = 1;
        }
        else if (moveX != 0 || moveZ != 0)
        {
            audioSource.loop = true;
            audioSource.Play();
            anim.Play("Walking");
            anim.speed = 1;
        }
        else
        {
            audioSource.loop = false;
            audioSource.Stop();
            anim.Play("Idle");
            anim.speed = 1;
        }

        rb.linearVelocity = newVelocity;

        if (jump)
        {
            Jump();
            anim.Play("Idle");
            anim.speed = 1;
        }

        if (sprint)
        {
            SprintSpeed();
            anim.Play("Walking");
            anim.speed = 2;
        }
        else
        {
            WalkingSpeed();
        }

        if (dash && !dashTimer.IsRunning && !sc.Shifting)
        {
            dashTimer.StartTimer(stats.DashCooldownTime);
            Dash();
            combat.AttackDash();
            anim.Play("Walking");
            anim.speed = 1;
        }

        if (slam && !IsSlamming && !sc.Shifting)
        {
            Slam();
            combat.AttackSlam();
            anim.Play("Idle");
            anim.speed = 1;
        }

        if (grounded)
        {
            IsSlamming = false;
        }
    }

    private void WalkingSpeed()
    {
        currentSpeed = stats.MoveSpeed;
    }

    private void SprintSpeed()
    {
        currentSpeed = stats.MoveSpeed * stats.SprintMultiplier;
    }

    private void Dash()
    {
        Vector3 dashVelocity = new Vector3(stats.DashForce * DirectionX, 0, 0);

        //rb.AddForce(dashVelocity, DashForceMode);
        rb.linearVelocity = dashVelocity;
    }

    private void Slam()
    {
        Vector3 slamVelocity = new Vector3(0, -stats.SlamForce, 0);

        rb.AddForce(slamVelocity, SlamForceMode);
        IsSlamming = true;
    }

    private void SetDirectionLeft()
    {
        DirectionX = -1;

        float xScale = Math.Abs(transform.localScale.x) * -1;

        Vector3 scale = transform.localScale;

        scale.x = xScale;

        transform.localScale = scale;
    }

    private void SetDirectionRight()
    {
        DirectionX = 1;

        float xScale = Math.Abs(transform.localScale.x);

        Vector3 scale = transform.localScale;

        scale.x = xScale;

        transform.localScale = scale;
    }

    private void Jump()
    {
        if (JumpOk())
        {
            //rb.linearVelocity = new Vector3(rb.linearVelocity.x, JumpForce, rb.linearVelocity.z);
            rb.AddForce(new Vector3(rb.linearVelocity.x, JumpForce, rb.linearVelocity.z), JumpForceMode);

            Jumps -= 1;
        }
    }

    private bool JumpOk()
    {
        return (Jumps > 0);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Jumps = MaxJumps;
            grounded = true;
            LastObject = other.gameObject;
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            Jumps = MaxJumps;
            grounded = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("DeathFloor"))
        {
            transform.position = LastObject.transform.position + new Vector3(0, 1, 0);
            rb.linearVelocity = new Vector3(0, 0, 0);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Jumps -= 1;
            grounded = false;
        }
    }
}