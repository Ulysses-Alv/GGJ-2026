using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugLinterna : MonoBehaviour
{
    public DynamoLeverVisual DynamoLeverVisual;

    private XRIInputActions inputActions;

    private void Awake()
    {
        inputActions = new XRIInputActions();
    }

    private void OnEnable()
    {
        inputActions.PC.Rotate.performed += OnRotate;
        inputActions.Enable();
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        DynamoLeverVisual.PlayImpulse();
    }

    private void OnDisable()
    {
        inputActions.PC.Rotate.performed -= OnRotate;
        inputActions.Disable();
    }
}