using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerJumpScare : MonoBehaviour, IJumpscare
{
    [SerializeField] private GameObject jumpscare;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource source;
bool isTriggered;
public static PlayerJumpScare instance;
public void Awake(){
instance = this;
}
    public void TriggerJumpscare()
    {
if(isTriggered) return;
isTriggered = true;
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