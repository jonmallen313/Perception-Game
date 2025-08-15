using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Stats stats;

    public KeyCode InteractKeyCode;
    private bool interacting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        interacting = Input.GetKey(InteractKeyCode);
    }

    private void LootTarget(Loot target)
    {
        stats.Hp.Current += target.DrainEnergy();
    }

    private void OnTriggerStay(Collider other)
    {
        if (interacting)
        {
            switch (other.gameObject.tag)
            {
                case "Loot":
                    Loot otherLoot = other.gameObject.GetComponent<Loot>();

                    if (!otherLoot.Looted)
                    {
                        LootTarget(otherLoot);
                    }

                    break;
            }
        }
    }
}