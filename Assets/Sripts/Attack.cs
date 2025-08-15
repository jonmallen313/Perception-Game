using UnityEngine;

public class Attack : MonoBehaviour
{
    public float Atk;
    public float Dur;

    private Timer durationTimer;
    public float Duration;

    public float Knockback;

    private Rigidbody rb;
    public Vector3 Velocity;

    private bool timerStarted;

    public ForceMode MeleeForceMode;

    public bool FollowTarget;
    private GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerStarted)
        {
            CheckDuration();
        }
        else
        {
            CheckSlamEnd();
        }

        CheckDurability();
        ProjectAttack();

        if (FollowTarget)
        {
            TrackTarget();
        }
    }

    public void Initialize(float atk, float dur, float duration, Vector3 velocity, float scale, ForceMode forceMode, bool visible, GameObject targetIn, bool slam)
    {
        Atk = atk;
        Dur = dur;
        Duration = duration;
        Velocity = velocity;
        transform.localScale *= scale;
        MeleeForceMode = forceMode;

        if (visible)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
        
        durationTimer = gameObject.AddComponent<Timer>();

        if (!slam)
        {
            durationTimer.StartTimer(Duration);
            timerStarted = true;
        }

        FollowTarget = true;
        target = targetIn;
    }    
    
    public void Initialize(float atk, float dur, float duration, Vector3 velocity, float scale, ForceMode forceMode, bool visible)
    {
        Atk = atk;
        Dur = dur;
        Duration = duration;
        Velocity = velocity;
        transform.localScale *= scale;
        MeleeForceMode = forceMode;

        if (visible)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
        
        durationTimer = gameObject.AddComponent<Timer>();
        durationTimer.StartTimer(Duration);
        timerStarted = true;
    }

    private void CheckDurability()
    {
        if (Dur <= 0)
        {
            //Destroy(gameObject);
        }
    }

    private void TrackTarget()
    {
        Vector3 targetPos = target.transform.position;
        
        transform.position = targetPos;
    }

    private void CheckDuration()
    {
        if (!durationTimer.IsRunning)
        {
            Destroy(gameObject);
        }
    }

    private void ProjectAttack()
    {
        rb.AddForce(Velocity, MeleeForceMode);
    }

    private void CheckSlamEnd()
    {
        PlayerMovementAlt movement = target.GetComponent<PlayerMovementAlt>();
        if (!movement.IsSlamming)
        {
            durationTimer.StartTimer(Duration);
            timerStarted = true;
        }
    }
}