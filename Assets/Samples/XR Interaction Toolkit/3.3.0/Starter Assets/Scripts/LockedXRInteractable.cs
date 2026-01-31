using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LockedXRInteractable : XRGrabInteractable
{
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        if (isSelected)
            return interactorsSelecting == interactor;

        return base.IsSelectableBy(interactor);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
    }
}

