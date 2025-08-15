using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ConversationManager : MonoBehaviour
{
    public GameObject dialogueUI;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI bodyText;
    public GameObject speakerImage;

    public static bool IsDialogueActive = false;

    private bool lineFullyShown = false;

    private List<DialogueLine> currentConversation;
    private int index = 0;
    private bool isTyping = false;
    private Coroutine typeCoroutine;

    [SerializeField]
    public float typeSpeed = 0.02f;

    [SerializeField]
    string nextScene;

    string convo;

    public GameObject vega;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            // handle forward motion input while typing
            if (isTyping)
            {
                StopCoroutine(typeCoroutine);
                isTyping = false;

                bodyText.text = currentConversation[index].text;
                lineFullyShown = true;
                if (index >= currentConversation.Count)
                {
                    EndConversation();
                    return;
                }
                 
            } else if (lineFullyShown)
            {
                index++;
                NextLine();
            }
        }
    }

    public void StartConversation(string conversationID)
    {
        convo = conversationID;
        IsDialogueActive = true;
        currentConversation = ConversationLoader.GetConversation(conversationID);
        index = 0;
        dialogueUI.SetActive(true);
        speakerImage.SetActive(true);
        NextLine();
    }

    public void EndConversation()
    {
        dialogueUI.SetActive(false);
        speakerImage.SetActive(false);
        ConversationManager.IsDialogueActive = false;
        if (convo == "beatrix_path_fight")
        {

            SceneManager.LoadScene("MainMenu");

        }
        else if (convo == "beatrix_path_pfight")
        {
            return;
        }
        else if(convo == "vega_path_pfight")
        {
            return;
        }else if(convo == "vega_path_fight")
        {
            vega.SetActive(true);
        }
        else if (convo == "lvl4")
        {
            return;
        }
        else
            SceneManager.LoadScene(nextScene);
    }

    private void NextLine()
    {
        // handle end conversation
        if (index >= currentConversation.Count)
        {
            EndConversation();
            return;
        }

        // get next line
        speakerText.text = currentConversation[index].speaker;
        typeCoroutine = StartCoroutine(TypeText(currentConversation[index].text));
        
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        lineFullyShown = false;
        bodyText.text = "";

        foreach (char c in text)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        lineFullyShown = true;
        
    }
}
