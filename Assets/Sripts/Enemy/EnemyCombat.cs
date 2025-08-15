using System;
using System.Collections;
using Shit;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyCombat : MonoBehaviour
{
    private Stats stats;
    private EnemyMovement movement;
    private ShiftConnection shiftConnection;
    private SpriteRenderer healthbar;

    public GameObject Loot;

    [Range(0, 5)] public float DamageCooldown = 1;
    private Timer damageCooldownTimer;
    private bool invulnerable;

    public float TimeStopDuration;

    public bool isVega;

    public AttackStats MeleeAttack;
    public AttackStats RangedAttack;

    public float MeleeRange;

    private void Awake()
    {
        stats = GetComponent<Stats>();
        movement = GetComponent<EnemyMovement>();

        MeleeAttack.SetTimer(gameObject.AddComponent<Timer>());
        RangedAttack.SetTimer(gameObject.AddComponent<Timer>());

        shiftConnection = GetComponent<ShiftConnection>();
    }

    private void OnDestroy()
    {
        if (isVega)
        {
            ConversationManager convo = GameObject.Find("convoobj3").GetComponent<ConversationManager>();
            convo.StartConversation("vega_path_ending");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damageCooldownTimer = gameObject.AddComponent<Timer>();
        healthbar = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        healthbar.color = Color.red;
        healthbar.transform.localScale = new Vector3(stats.Hp.Current / stats.Hp.CurrentMax,
            healthbar.transform.localScale.y, healthbar.transform.localScale.z);

        if (stats.Hp.Current <= 0)
        {
            GameObject instance = Instantiate(Loot, transform.position, Quaternion.identity);

            instance.GetComponent<Loot>().Initialize(stats.Hp.Max);

            Time.timeScale = 1;
            Destroy(gameObject);
        }

        invulnerable = damageCooldownTimer.IsRunning;

        if (movement.FollowTarget)
        {
            if (movement.Shifter)
            {
                bool inRange = Math.Abs(movement.GetTargetPosition().x - transform.position.x) < MeleeRange;

                if (inRange)
                {
                    if (!MeleeAttack.GetTimer().IsRunning && !shiftConnection.Shifting)
                    {
                        AttackMelee();
                        MeleeAttack.GetTimer().StartTimer(MeleeAttack.AtkCooldown * stats.AtkSpeed);
                    }
                }
            }
            else
            {
                if (!RangedAttack.GetTimer().IsRunning && !shiftConnection.Shifting)
                {
                    AttackRanged();
                    RangedAttack.GetTimer().StartTimer(RangedAttack.AtkCooldown * stats.AtkSpeed);
                }
            }
        }
    }

    private void AttackMelee()
    {
        Vector3 meleeOffset = new Vector3(MeleeAttack.Offset.x * movement.DirectionX, MeleeAttack.Offset.y,
            MeleeAttack.Offset.z);

        Vector3 spawnPos = transform.position + meleeOffset;

        Vector3 meleeVelocity = new Vector3(MeleeAttack.Speed * movement.DirectionX, 0, 0);

        GameObject instance = Instantiate(MeleeAttack.Projectile, spawnPos, Quaternion.identity);

        Attack instanceAttack = instance.GetComponent<Attack>();

        instanceAttack.Initialize(stats.AtkPower + instanceAttack.Atk, MeleeAttack.Durability, MeleeAttack.Duration,
            meleeVelocity, MeleeAttack.Scale, MeleeAttack.ForceMode, MeleeAttack.Visible);
    }

    private void AttackRanged()
    {
        Vector3 rangedOffset = new Vector3(RangedAttack.Offset.x * movement.DirectionX, RangedAttack.Offset.y,
            RangedAttack.Offset.z);

        Vector3 spawnPos = transform.position + rangedOffset;

        Vector3 rangedVelocity = new Vector3(RangedAttack.Speed * movement.DirectionX, 0, 0);

        GameObject instance = Instantiate(RangedAttack.Projectile, spawnPos, Quaternion.identity);

        Attack instanceAttack = instance.GetComponent<Attack>();

        instanceAttack.Initialize(stats.AtkPower + instanceAttack.Atk, RangedAttack.Durability, RangedAttack.Duration,
            rangedVelocity, RangedAttack.Scale, RangedAttack.ForceMode, RangedAttack.Visible);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttack") && !invulnerable)
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
    }

    IEnumerator StopTime(float time)
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(time);

        Time.timeScale = 1;
    }
}