using UnityEngine;

public class TempTrigger : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindFirstObjectByType<DeathScreenManager>().ShowDeathScreen();
        }
    }
}
