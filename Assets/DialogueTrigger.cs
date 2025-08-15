using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField]
    string cScene;

    public ConversationManager vega;
    

    bool state = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") && state)
        {
            

            //Toggle start convo on collide with floor panel !!
            vega.StartConversation(cScene);
            if (cScene == "beatrix_path_fight")
            {
               // vega.SetActive(true);
            }
            state = false;
            
        }
    }
}
