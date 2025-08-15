using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target; // Player
    public Vector3 offset = new Vector3(0, 0, -10);
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(target.position.x, target.position.y, -10);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
