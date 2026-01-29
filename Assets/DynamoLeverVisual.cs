using UnityEngine;
using DG.Tweening;
public class DynamoLeverVisual : MonoBehaviour
{
    public Transform lever;
    public enum Axis
    {
        X,
        Y,
        Z
    }

    [SerializeField] private Axis axis = Axis.Z;
    [SerializeField] private float visualRotationPerImpulse = 45f;
    [SerializeField] private float duration = 0.25f;

    private Tween currentTween;

    public void PlayImpulse(bool isRight = true)
    {
        currentTween?.Kill();

        float direction = isRight ? 1f : -1f;
        Vector3 rotation = Vector3.zero;

        if (axis == Axis.X) rotation.x = visualRotationPerImpulse * direction;
        if (axis == Axis.Y) rotation.y = visualRotationPerImpulse * direction;
        if (axis == Axis.Z) rotation.z = visualRotationPerImpulse * direction;

        currentTween = lever.DOLocalRotate(
            transform.localEulerAngles + rotation,
            duration
        ).SetEase(Ease.OutCubic);
    }
}
