using UnityEngine;

public class ShiftConnection : MonoBehaviour
{
    private PerspectiveShift pShift;
    
    public bool Shifting;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pShift = GameObject.Find("Main Camera").GetComponent<PerspectiveShift>();
    }

    // Update is called once per frame
    void Update()
    {
        Shifting = pShift.IsIn3DMode();
    }
}
