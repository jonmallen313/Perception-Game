using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;

    void OnTriggerEnter(Collider other)
    {
        if (isOpen) return;

        if (other.CompareTag("Player"))
        {
            var inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.hasKey && inventory.key == 1)
            {
                OpenDoor();
                // Optionally consume key:
                inventory.hasKey = false;
                inventory.key = 0;
            }
            else
            {
                Debug.Log("You need a key to open this door.");
            }
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Door opened!");

        gameObject.SetActive(false); // simple door disappears
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
