using System;
using Shit;
using UnityEngine;
using UnityEngine.Serialization;

public class Stats : MonoBehaviour
{
    public Stat Hp;

    public float MoveSpeed;
    public float SprintMultiplier;
    
    public float DashForce;
    public float DashCooldownTime;

    public float SlamForce;
    
    public float AtkPower;
    public float AtkSpeed;
    
    public float Def;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CutHpCap();
    }

    private void CutHpCap()
    {
        if (Hp.Current > Hp.CurrentMax)
        {
            Hp.Current = Hp.CurrentMax;
        }

        if (Hp.Current < 0)
        {
            Hp.Current = 0;
        }
    }
}
