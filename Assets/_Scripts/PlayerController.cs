using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform cameraTransform;

    private XRIInputActions inputActions;
    private CharacterController characterController;
    private Vector2 moveInput;
    private float cameraPitch;

    private void Awake()
    {
        inputActions = new XRIInputActions();
        characterController = GetComponent<CharacterController>();
    }
    Action updateActions;
    private void Start()
    {
        updateActions += HandleMovement;
        if (!GameMode.isVR)
        {
            updateActions += HandleMouseLook;
        }
    }
    private void OnEnable()
    {
        inputActions.XRILeftLocomotion.Move.performed += OnMove;
        inputActions.XRILeftLocomotion.Move.canceled += OnMove;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.XRILeftLocomotion.Move.performed -= OnMove;
        inputActions.XRILeftLocomotion.Move.canceled -= OnMove;
        inputActions.Disable();
    }

    private void Update()
    {
        updateActions.Invoke();
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        direction = transform.TransformDirection(direction);
        characterController.Move(direction * moveSpeed * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null)
            return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * Sensi.sens;

        cameraPitch -= mouseDelta.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseDelta.x);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
public interface IInteractable
{
    void Interact();
}
