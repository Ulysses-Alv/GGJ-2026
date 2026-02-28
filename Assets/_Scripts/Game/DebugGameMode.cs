using UnityEngine;
#if UNITY_EDITOR

public class DebugGameMode : MonoBehaviour
{
    [SerializeField] bool isVR = false;

    private void Awake()
    {
        GameMode.isVR = isVR;
    }
}
#endif
