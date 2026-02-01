using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class RedButtonHallway : RedButtonBase
{
    [SerializeField] AudioSource ambientSource;
    [SerializeField] AudioClip darkSound;
    bool interacted = false;
    public override void Interact()
    {
        if (interacted) return;

        interacted = true;
        Debug.Log("Interacted");
        LightController.instance.SwitchLights(false);
        ambientSource.DOFade(0f, 2f).OnComplete(
            () =>
            {
                ambientSource.clip = darkSound;
                ambientSource.DOFade(1f, 1f);
                ambientSource.Play();
            }
        );
        AIManager.Instance.EnableActivation();

    }
}

public abstract class RedButtonBase : MonoBehaviour, IInteractable
{
    private void Start()
    {
        GetComponentInChildren<XRSimpleInteractable>().selectEntered.AddListener(OnSelectEntered);
    }
    private void OnSelectEntered(SelectEnterEventArgs arg0)
    {
        this.Interact();
    }

    public abstract void Interact();

}