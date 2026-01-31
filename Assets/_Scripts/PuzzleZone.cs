using UnityEngine;

public class PuzzleZone : MonoBehaviour
{
    [SerializeField] private GameObject handle;

    private bool isActive = false;

    private void Start()
    {
        handle.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isActive) return;

        if (other.CompareTag("Player"))
        {
            if (PlayerPuzzles.Instance.hasHandle)
            {
                isActive = true;
                handle.SetActive(true);
            }
        }
    }
}