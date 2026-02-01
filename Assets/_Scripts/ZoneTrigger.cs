using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] private GameZones gameZones;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AIManager.Instance.SetPlayerZone(gameZones);
        }
    }

}

public enum GameZones
{
    SafeZone,
    Main,
    Electricity,
    HallwayEnd
}
