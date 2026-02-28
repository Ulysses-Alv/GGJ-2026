using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CreditScreen : MonoBehaviour
{
    [SerializeField] private float scrollDuration = 10f;
    [SerializeField] private float scrollDistance = 1000f;
    void Start()
    {
        transform.DOLocalMoveY(scrollDistance, scrollDuration).SetEase(Ease.Linear).OnComplete(OnCreditsFinished);          
    }

    void OnCreditsFinished()
    {
        // Load the next scene when credits are finished
        SceneManager.LoadScene("Menu");
    }
}
