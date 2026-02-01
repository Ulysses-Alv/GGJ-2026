using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FootStepModule : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepDistance = 1.8f;
    [SerializeField] private float minSpeed = 0.1f;

    private CharacterController characterController;
    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float distanceAccumulated;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        Vector3 delta = transform.position - lastPosition;
        float horizontalDistance = new Vector3(delta.x, 0f, delta.z).magnitude;

        float speed = horizontalDistance / Time.deltaTime;

        if (speed > minSpeed && characterController.isGrounded)
        {
            distanceAccumulated += horizontalDistance;

            if (distanceAccumulated >= stepDistance)
            {
                PlayFootstep();
                distanceAccumulated = 0f;
            }
        }
        else
        {
            distanceAccumulated = 0f;
        }

        lastPosition = transform.position;
    }

    private void PlayFootstep()
    {
        if (footstepClips.Length == 0)
            return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip);
    }
}
