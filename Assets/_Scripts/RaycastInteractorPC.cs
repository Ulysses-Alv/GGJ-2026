using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastInteractorPC : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private GameObject pressE;

    private XRIInputActions inputActions;
    private IInteractable currentInteractable;

    private void Awake()
    {
        inputActions = new XRIInputActions();
    }

    private void OnEnable()
    {
        inputActions.PC.Interact.performed += OnInteract;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.PC.Interact.performed -= OnInteract;
        inputActions.Disable();
    }

    private void Update()
    {
        currentInteractable = null;

        if (Physics.Raycast(
            cameraTransform.position,
            cameraTransform.forward,
            out RaycastHit hit,
            interactDistance,
            interactLayer)
            )
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                currentInteractable = interactable;
            }
        }

        pressE.SetActive(currentInteractable != null);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        currentInteractable?.Interact();
    }
}
