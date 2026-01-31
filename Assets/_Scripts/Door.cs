using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    private UnityEvent doorOpenEvent = new();
    private UnityEvent doorCloseEvent = new();

    public enum DoorDir
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }

    [SerializeField] private DoorDir doorDirection;
    [SerializeField] private float openDuration = 1f;
    [SerializeField] private Transform DoorVisual;
    private bool isOpen;
    private Vector3 closedPosition;


    private void Awake()
    {
        closedPosition = DoorVisual.localPosition;
        doorOpenEvent.AddListener(OpenDoor);
        doorCloseEvent.AddListener(CloseDoor);
    }

    [ContextMenu("Open Sesamo!")]
    public void TriggerOpen()
    {
        if (isOpen)
            return;

        doorOpenEvent.Invoke();
    }

    [ContextMenu("Close Sesamo!")]
    public void TriggerClose()
    {
        if (!isOpen)
            return;

        doorCloseEvent.Invoke();
    }

    private void OpenDoor()
    {
        Vector3 finalPos = closedPosition + GetMoveOffset();
        DoorVisual.DOLocalMove(finalPos, openDuration).SetEase(Ease.OutQuad);
        isOpen = true;
    }

    private void CloseDoor()
    {
        DoorVisual.DOLocalMove(closedPosition, openDuration);
        isOpen = false;
    }

    private Vector3 GetMoveOffset()
    {
        return doorDirection switch
        {
            DoorDir.Up => Vector3.up * 2f,
            DoorDir.Down => Vector3.down * 2f,
            DoorDir.Left => Vector3.left * 1f,
            DoorDir.Right => Vector3.right * 1f,
            DoorDir.Forward => Vector3.forward * 1f,
            DoorDir.Backward => Vector3.back * 1f,
            _ => Vector3.zero
        };
    }
}
