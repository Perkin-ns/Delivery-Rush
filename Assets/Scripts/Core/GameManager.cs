using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string gameOverScene = "GameOver";

    private void Start()
    {
        DeliveryManager.Instance.OnGameOver += OnGameOver;
    }

    private void OnGameOver()
    {
        PlayerPrefs.SetInt("FinalScore", DeliveryManager.Instance.Score);
        PlayerPrefs.SetInt("DeliveriesCompleted", DeliveryManager.Instance.DeliveriesCompleted);
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameOverScene);
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance != null)
            DeliveryManager.Instance.OnGameOver -= OnGameOver;
    }
}
