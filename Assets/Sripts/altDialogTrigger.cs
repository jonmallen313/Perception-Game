using UnityEngine;

public class altDialogTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ConversationManager convo = GameObject.Find("convo").GetComponent<ConversationManager>();
        convo.StartConversation("lvl3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
