using UnityEngine;

public class RedButton : RedButtonBase
{
    [SerializeField] private Door hallwayDoor, electricityDoor;
    [SerializeField] private PuzzleZone puzzle;

    public override void Interact()
    {
        puzzle.StopAllCoroutines();
        hallwayDoor.TriggerOpen();
        electricityDoor.TriggerOpen();
    }
}
