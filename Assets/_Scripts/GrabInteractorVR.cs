using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabInteractorVR : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private IInteractable interactable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        interactable = GetComponent<IInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactable.Interact();
    }
}

