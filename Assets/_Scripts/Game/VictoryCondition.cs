using UnityEngine;
using UnityEngine.SceneManagement;
public class VictoryCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player has reached the victory condition!");
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Credits");
        }
    }
}