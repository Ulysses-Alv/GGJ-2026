using UnityEngine;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] DynamoLeverVisual visual;
    private void Awake()
    {
        visual = GetComponent<DynamoLeverVisual>();
    }
    public void Interact()
    {
        visual.PlayImpulse();
    }
}