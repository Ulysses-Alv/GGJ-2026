using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject vrPlayer, pcPlayer;

    private void Start()
    {
        GameObject playerPrefab = GameMode.isVR ? vrPlayer : pcPlayer;
        GameObject otherPrefab = !GameMode.isVR ? vrPlayer : pcPlayer;

        playerPrefab.SetActive(true);
        playerPrefab.transform.SetPositionAndRotation(transform.position, transform.rotation);

        Destroy(otherPrefab);
    }
}