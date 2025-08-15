using System.Collections;
using UnityEngine;

public class CameraFlip : MonoBehaviour
{

    public float flipDuration = 1f;
    private bool isFlipped = false;
    private bool isFlipping = false;

    public Transform cameraTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void FlipCamera()
    {
        if (isFlipping) return;
        StartCoroutine(FlipCoroutine());
    }

    private IEnumerator FlipCoroutine()
    {
        isFlipping = true;
        float timeElapsed = 0f;
        Quaternion starRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + (isFlipped ? -180f : 180f),
            transform.eulerAngles.z);

        while (timeElapsed < flipDuration)
        {
            transform.rotation = Quaternion.Slerp(starRot, endRot, timeElapsed / flipDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
        isFlipped = !isFlipped;
        isFlipping = false;
    }
}
