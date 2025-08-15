using System.Collections;
using Shit;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombat : MonoBehaviour
{
    private Stats stats;
    private PlayerMovementAlt movement;
    private ShiftConnection shiftConnection;
    private AudioSource audioSource;

    [Range(0, 5)] public float DamageCooldown = 1;
    private Timer damageCooldownTimer;
    public bool Invulnerable;

    public float TimeStopDuration;

    public AttackStats MeleeAttack;
    public AttackStats RangedAttack;
    public AttackStats SlamAttack;
    public AttackStats DashAttack;
    
    private Timer dashTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = GetComponent<Stats>();
        movement = GetComponent<PlayerMovementAlt>();
        shiftConnection = GetComponent<ShiftConnection>();
        audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.playOnAwake = false;

        damageCooldownTimer = gameObject.AddComponent<Timer>();
        dashTimer = gameObject.AddComponent<Timer>();

        MeleeAttack.SetTimer(gameObject.AddComponent<Timer>());
        RangedAttack.SetTimer(gameObject.AddComponent<Timer>());
    }

    // Update is called once per frame
    void Update()
    {
        if (stats.Hp.Current <= 0)
        {
            // TODO death screen
            Debug.Log($"PLAYER HAS FALLEN");
        }

        Invulnerable = damageCooldownTimer.IsRunning || dashTimer.IsRunning;

        MeleeAttack.IsAttacking = Input.GetKeyDown(MeleeAttack.AttackKey);
        RangedAttack.IsAttacking = Input.GetKeyDown(RangedAttack.AttackKey);

        if (MeleeAttack.IsAttacking && !MeleeAttack.GetTimer().IsRunning && !shiftConnection.Shifting)
        {
            if (!movement.IsSlamming)
            {
                AttackMelee();
                MeleeAttack.GetTimer().StartTimer(MeleeAttack.AtkCooldown * stats.AtkSpeed);
            }
        }

        if (RangedAttack.IsAttacking && !RangedAttack.GetTimer().IsRunning && !shiftConnection.Shifting)
        {
            AttackRanged();
            RangedAttack.GetTimer().StartTimer(RangedAttack.AtkCooldown * stats.AtkSpeed);
        }
    }

    /// <summary>
    /// Melee attack
    /// Large dmg radius
    /// Close range
    /// No movement
    /// </summary>
    private void AttackMelee()
    {
        audioSource.pitch = 1;
        
        audioSource.clip = MeleeAttack.AttackSound;
        audioSource.Play();
        
        Vector3 meleeOffset = new Vector3(MeleeAttack.Offset.x * movement.DirectionX, MeleeAttack.Offset.y,
            MeleeAttack.Offset.z);

        Vector3 spawnPos = transform.position + meleeOffset;

        Vector3 meleeVelocity = new Vector3(MeleeAttack.Speed * movement.DirectionX, 0, 0);

        GameObject instance = Instantiate(MeleeAttack.Projectile, spawnPos, Quaternion.identity);

        Attack instanceAttack = instance.GetComponent<Attack>();

        instanceAttack.Initialize((stats.AtkPower + instanceAttack.Atk) * MeleeAttack.AtkPowerMultiplier, MeleeAttack.Durability, MeleeAttack.Duration,
            meleeVelocity, MeleeAttack.Scale, MeleeAttack.ForceMode, MeleeAttack.Visible);


    }

    /// <summary>
    /// Ranged attack
    /// Small projectiles
    /// Fast movement
    /// </summary>
    private void AttackRanged()
    {
        Vector3 rangedOffset = new Vector3(RangedAttack.Offset.x * movement.DirectionX, RangedAttack.Offset.y,
            RangedAttack.Offset.z);

        Vector3 spawnPos = transform.position + rangedOffset;

        Vector3 rangedVelocity = new Vector3(RangedAttack.Speed * movement.DirectionX, 0, 0);

        GameObject instance = Instantiate(RangedAttack.Projectile, spawnPos, Quaternion.identity);

        Attack instanceAttack = instance.GetComponent<Attack>();

        instanceAttack.Initialize((stats.AtkPower + instanceAttack.Atk) * RangedAttack.AtkPowerMultiplier, RangedAttack.Durability, RangedAttack.Duration,
            rangedVelocity, RangedAttack.Scale, RangedAttack.ForceMode, RangedAttack.Visible);
        
        audioSource.pitch = 2;
        
        audioSource.clip = MeleeAttack.AttackSound;
        audioSource.Play();
    }

    /// <summary>
    /// Activates when slammed on the ground
    /// Big area dmg
    /// No movement
    /// </summary>
    public void AttackSlam()
    {
        Vector3 slamOffset = new Vector3(SlamAttack.Offset.x * movement.DirectionX, SlamAttack.Offset.y,
            SlamAttack.Offset.z);

        Vector3 spawnPos = transform.position + slamOffset;
        
        Vector3 slamVelocity = new Vector3(SlamAttack.Speed * movement.DirectionX, 0, 0);

        GameObject instance = Instantiate(SlamAttack.Projectile, spawnPos, Quaternion.identity);

        Attack instanceAttack = instance.GetComponent<Attack>();

        instanceAttack.Initialize((stats.AtkPower + instanceAttack.Atk) * SlamAttack.AtkPowerMultiplier, SlamAttack.Durability, SlamAttack.Duration,
            slamVelocity, SlamAttack.Scale, SlamAttack.ForceMode, SlamAttack.Visible, gameObject, true);
        
        audioSource.clip = MeleeAttack.AttackSound;
        audioSource.Play();
    }

    /// <summary>
    /// Dash attack for fast moving
    /// Medium area
    /// Follow player
    /// </summary>
    public void AttackDash()
    {
        Vector3 dashOffset = new Vector3(DashAttack.Offset.x * movement.DirectionX, DashAttack.Offset.y,
            DashAttack.Offset.z);

        Vector3 spawnPos = transform.position + dashOffset;


        Vector3 dashVelocity = new Vector3(DashAttack.Speed * movement.DirectionX, 0, 0);

        GameObject instance = Instantiate(DashAttack.Projectile, spawnPos, Quaternion.identity);

        Attack instanceAttack = instance.GetComponent<Attack>();

        instanceAttack.Initialize((stats.AtkPower + instanceAttack.Atk) * DashAttack.AtkPowerMultiplier, DashAttack.Durability, DashAttack.Duration,
            dashVelocity, DashAttack.Scale, DashAttack.ForceMode, DashAttack.Visible, gameObject, false);
        
        MakeInvulnerable(DashAttack.Duration);
        
        audioSource.clip = MeleeAttack.AttackSound;
        audioSource.Play();
    }

    public void MakeInvulnerable(float duration)
    {
        
        
        dashTimer.StartTimer(duration);
        Invulnerable = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyAttack") && !Invulnerable)
        {
            Attack otherStats = other.gameObject.GetComponent<Attack>();

            if (otherStats.Atk > stats.Def)
            {
                stats.Hp.Current -= otherStats.Atk - stats.Def;
                otherStats.Dur -= 1;

                StartCoroutine(StopTime(TimeStopDuration));

                damageCooldownTimer.StartTimer(DamageCooldown);


                if (otherStats.Dur <= 0)
                {
                    Destroy(other.gameObject);
                }
            }
        }

        IEnumerator StopTime(float time)
        {
            Time.timeScale = 0;

            yield return new WaitForSecondsRealtime(time);

            Time.timeScale = 1;
        }
    }
}