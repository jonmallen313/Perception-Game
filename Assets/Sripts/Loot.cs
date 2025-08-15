using UnityEngine;

public class Loot : MonoBehaviour
{
    private SpriteRenderer barSR;
    
    private float Energy;
    // Add REM-Note Here

    public bool Looted;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        barSR = GetComponentInChildren<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(float energy)
    {
        Energy = energy;
    }    

    public float DrainEnergy()
    {
        float energy = Energy;
        
        Looted = true;
        Energy = 0;
        
        barSR.color = Color.black;
        return energy;
    }
}
