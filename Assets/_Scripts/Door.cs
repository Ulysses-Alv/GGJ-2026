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

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip open, close;

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
        audioSource.PlayOneShot(open);
    }

    private void CloseDoor()
    {
        audioSource.PlayOneShot(close);
        DoorVisual.DOLocalMove(closedPosition, openDuration);
        isOpen = false;
    }

    private Vector3 GetMoveOffset()
    {
        return doorDirection switch
        {
            DoorDir.Up => Vector3.up * 2f,
            DoorDir.Down => Vector3.down * 2f,
            DoorDir.Left => Vector3.left * 2f,
            DoorDir.Right => Vector3.right * 2f,
            DoorDir.Forward => Vector3.forward * 2f,
            DoorDir.Backward => Vector3.back * 1f,
            _ => Vector3.zero
        };
    }
}
