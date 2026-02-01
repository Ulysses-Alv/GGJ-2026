using UnityEngine;

public class CrankRotateProxy : MonoBehaviour
{
    [SerializeField] private Transform grabProxy;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;
    [SerializeField] private float rotationMultiplier = 1f;
    [SerializeField]
    private CrankRotator rotator;
    private Quaternion lastProxyRotation;

    private void Start()
    {
        lastProxyRotation = grabProxy.rotation;
    }

    private void LateUpdate()
    {
        if (!rotator.isGrabbed) return;

        Quaternion currentRotation = grabProxy.rotation;
        Quaternion delta = currentRotation * Quaternion.Inverse(lastProxyRotation);

        float angle;
        Vector3 axis;
        delta.ToAngleAxis(out angle, out axis);

        float signedAngle = angle * Mathf.Sign(Vector3.Dot(axis, grabProxy.TransformDirection(rotationAxis)));

        transform.Rotate(rotationAxis, signedAngle * rotationMultiplier, Space.Self);

        lastProxyRotation = currentRotation;
    }
}
