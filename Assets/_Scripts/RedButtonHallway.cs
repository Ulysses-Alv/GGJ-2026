using UnityEngine;
using UnityEngine.SceneManagement;

public class RedButtonHallway : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        LightController.instance.SwitchLights(false);

    }
}
