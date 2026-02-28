using UnityEngine;

public class HandlePuzzle : MonoBehaviour, IInteractable
{
    private void Start()
    {
        if (GameMode.isVR)
        {
            GetComponent<Collider>().enabled = false;
        }
    }
    public void Interact()
    {
        PlayerPuzzles.Instance.hasHandle = true;
        gameObject.SetActive(false);
    }
}
