using UnityEngine;

public class KeyDoorTrigger : MonoBehaviour
{
    public GameObject doorToOpen; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            PlayerInventory inv = other.GetComponent<PlayerInventory>();

            //Toggle start convo on collide with floor panel !!
            //FindObjectOfType<ConversationManager>().StartConversation("intro_scene");

            if (inv != null && inv.hasKey && inv.key == 1)
            {
                Debug.Log("Player has key. Opening door.");
                if (doorToOpen != null)
                    doorToOpen.SetActive(false); // Makes the door disappear
            }
            else
            {
                Debug.Log("Player needs a key to open this door.");
            }
        }
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
