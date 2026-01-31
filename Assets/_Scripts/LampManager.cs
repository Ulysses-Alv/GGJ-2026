using UnityEngine;

public class LampManager : MonoBehaviour
{
    [SerializeField] private LockedXRInteractable xRInteractable;
    private void Start()
    {
        xRInteractable.enabled = GameMode.isVR;
    }
}
