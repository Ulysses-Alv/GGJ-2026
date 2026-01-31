using UnityEngine;

public class PlayerPuzzles : MonoBehaviour
{
    internal static PlayerPuzzles Instance;

    public bool hasHandle;
    private void Awake()
    {
        Instance = this;
    }
}
