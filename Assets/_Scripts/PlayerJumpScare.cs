using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerJumpScare : MonoBehaviour, IJumpscare
{
    [SerializeField] private GameObject jumpscare;
    [SerializeField] private Animator animator;


    public void TriggerJumpscare()
    {
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