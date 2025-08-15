using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Stats stats;

    public bool Shifter;

    private GameObject target;

    public float DirectionX;

    public bool FollowTarget;

    public Vector3 TrackBuffer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<Stats>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > target.transform.position.y + TrackBuffer.y ||
            transform.position.y < target.transform.position.y - TrackBuffer.y)
        {
            FollowTarget = false;
        }
        else
        {
            FollowTarget = true;
        }

        float moveX = 0;
        float moveZ = 0;

        if (FollowTarget)
        {
            if (transform.position.x > target.transform.position.x + TrackBuffer.x)
            {
                moveX = -1;
                DirectionX = -1;
            }

            if (transform.position.x < target.transform.position.x - TrackBuffer.x)
            {
                moveX = 1;
                DirectionX = 1;
            }


            if (Shifter)
            {
                if (transform.position.z > target.transform.position.z + TrackBuffer.z)
                {
                    moveZ = -1;
                }

                if (transform.position.z < target.transform.position.z - TrackBuffer.z)
                {
                    moveZ = 1;
                }
            }
        }

        Vector3 movement = new Vector3(moveX * stats.MoveSpeed, 0, moveZ * stats.MoveSpeed);
        rb.linearVelocity = movement;
    }

    public Vector3 GetTargetPosition()
    {
        return target.transform.position;
    }
}