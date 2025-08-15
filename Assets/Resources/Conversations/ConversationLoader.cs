using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConversationLoader : MonoBehaviour
{
    public static ConversationList data;

    private void Awake()
    {
        LoadConversations();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // load conversations from json
    private void LoadConversations()
    {
        //load json
        TextAsset jsonFile = Resources.Load<TextAsset>("Conversations/conversations");

        if (jsonFile == null)
        {
            Debug.LogError("Could not find conversations.json in Resources.");
            return;
        }

        data = JsonUtility.FromJson<ConversationList>(jsonFile.text);

        if(data == null || data.conversations == null)
        {
            Debug.LogError("Failed to parse conversation data.");
            return;
        }


        Debug.Log("Loaded conversations:");
        foreach(var convo in data.conversations)
        {
            Debug.Log($"- {convo.id}");
        }
    }

    // return conversation by id
    public static List<DialogueLine> GetConversation(string id)
    {
        if (data == null)
        {
            // Try to load data now, just in case it hasn't been loaded yet
            TextAsset jsonFile = Resources.Load<TextAsset>("Conversations/conversations");
            if (jsonFile == null)
            {
                Debug.LogError("Could not find conversations.json in Resources/Conversations/");
                return new List<DialogueLine>();
            }

            data = JsonUtility.FromJson<ConversationList>(jsonFile.text);
            if (data == null || data.conversations == null)
            {
                Debug.LogError("Failed to parse conversation data.");
                return new List<DialogueLine>();
            }

            Debug.Log("Conversations loaded via fallback GetConversation()");
        }

        foreach (var convo in data.conversations)
        {
            if (convo.id == id)
            {
                return convo.lines;
            }
        }

        Debug.LogWarning($"Conversation '{id}' not found.");
        return new List<DialogueLine>();
    }
}
