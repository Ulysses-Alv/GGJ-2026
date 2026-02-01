using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject vrPlayer, pcPlayer;

    private void Start()
    {
        if (GameMode.isVR)
        {
            vrPlayer.SetActive(true);
            Destroy(pcPlayer);
        }
        else
        {
            pcPlayer.SetActive(true);
            Destroy(vrPlayer);
        }
    }

}

#if UNITY_EDITOR
#endif
