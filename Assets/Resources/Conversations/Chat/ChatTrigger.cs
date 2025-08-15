using UnityEngine;

public class ChatTrigger : MonoBehaviour
{
    [SerializeField] private WorldChatBox chatBox;
    [TextArea] public string message;
    [SerializeField] private float activationDistance = 3f; // 3 feet ~= 1 unit in Unity

    private GameObject player;
    private bool isActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (chatBox != null)
        {
            chatBox.Hide(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || chatBox == null)
            return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= activationDistance && !isActive)
        {
            chatBox.ShowMessage(message);
            isActive = true;
        }
        else if (distance > activationDistance && isActive)
        {
            chatBox.Hide();
            isActive = false;
        }
    }

    
}
