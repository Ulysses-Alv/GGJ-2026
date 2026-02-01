using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerJumpScare : MonoBehaviour, IJumpscare
{
    [SerializeField] private GameObject jumpscare;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource source;


    public void TriggerJumpscare()
    {
        source.Play();
        animator.SetTrigger("activate");
        Invoke(nameof(GoMenu), 2f);

    }
    private void GoMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
public interface IJumpscare
{
    void TriggerJumpscare();
}