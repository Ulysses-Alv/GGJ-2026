using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CrankRotator : MonoBehaviour
{
    [SerializeField] private Transform crankPivot, crankVisual;
    [SerializeField] private float rotationSpeed = 180f;

    private XRGrabInteractable grabInteractable;
    public bool isGrabbed;
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        transform.parent = crankPivot;
        transform.localPosition = Vector3.zero;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        isGrabbed = false;
        transform.parent = crankPivot;
        transform.localPosition = Vector3.zero;
        transform.localRotation = crankVisual.localRotation;
    }
    private void LateUpdate()
    {
        transform.localPosition = Vector3.zero;
    }
}


