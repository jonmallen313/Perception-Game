using UnityEngine;

public class CameraFlipTrigger : MonoBehaviour
{
    bool done = false;
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
        if(other.CompareTag("Player"))
        {
            Camera.main.GetComponent<PerspectiveShift>().Flip2DView();
            if (!done)
            {
                StartCoroutine(WaitAndDoSomething());
            }
            
        }
    }

    private System.Collections.IEnumerator WaitAndDoSomething()
    {
        done = true;
        yield return new WaitForSeconds(5f);

        done = false;

    }
}
