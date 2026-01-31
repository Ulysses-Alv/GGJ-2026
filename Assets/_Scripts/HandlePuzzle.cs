using UnityEngine;

public class HandlePuzzle : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        PlayerPuzzles.Instance.hasHandle = true;
        gameObject.SetActive(false);
    }
}
