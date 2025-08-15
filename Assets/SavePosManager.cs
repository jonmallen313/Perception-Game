using System;
using UnityEngine;

public class SavePosManager : MonoBehaviour
{
    private PlayerMovementAlt movement;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponentInParent<PlayerMovementAlt>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
