using UnityEngine;

public class RedButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Door hallwayDoor, electricityDoor;
    [SerializeField] private PuzzleZone puzzle;

    public void Interact()
    {
        puzzle.StopAllCoroutines();
        hallwayDoor.TriggerOpen();
        electricityDoor.TriggerOpen();
    }
}
