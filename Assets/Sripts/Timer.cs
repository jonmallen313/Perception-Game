using UnityEngine;

/// <summary>
/// A timer class used for duration and cooldown
/// </summary>
public class Timer : MonoBehaviour
{
    private float endTime;
    
    public bool IsRunning;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= endTime && IsRunning)
        {
            StopTimer();
        }
    }

    public void StartTimer(float duration)
    {
        endTime = Time.time + duration;
        
        IsRunning = true;
    }

    private void StopTimer()
    {
        endTime = 0;
        
        IsRunning = false;
    }
}
