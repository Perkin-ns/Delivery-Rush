using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string gameOverScene = "GameOver";

    private void Awake()
    {
        DeliveryManager.OnGameOver += OnGameOver;
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
        DeliveryManager.OnGameOver -= OnGameOver;
    }
}
