using UnityEngine;

public class SafeZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AIManager.Instance.OnPlayerEnteredSafeZone();
        }
    }
}
