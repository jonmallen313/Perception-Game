using System.Collections;
using TMPro;
using UnityEngine;

public class WorldChatBox : MonoBehaviour
{
    public TextMeshProUGUI chatText;
    public float typeSpeed = 0.02f;
    public float displayDuration = 3f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMessage(string message)
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        gameObject.SetActive(true);

        chatText.text = "";

        typingCoroutine = StartCoroutine(TypeMessage(message));

        
        
    }

    private IEnumerator TypeMessage(string message)
    {
        isTyping = true;
        chatText.text = "";
        
        foreach(char c in message)
        {
            chatText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;

        yield return new WaitForSeconds(displayDuration);
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
