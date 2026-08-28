using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string gameOverScene = "GameOver";

    private void Awake()
    {
        DeliveryManager.OnGameOver += OnGameOver;
    }

    private void OnGameOver()
    {
        IDeliveryService delivery = ServiceLocator.Get<IDeliveryService>();
        IPersistenceService save = ServiceLocator.Get<IPersistenceService>();

        save.SetInt("FinalScore", delivery.Score);
        save.SetInt("DeliveriesCompleted", delivery.DeliveriesCompleted);

        string name = save.GetString("PlayerName", "Player");
        ServiceLocator.Get<ILeaderboardService>().Add(name, delivery.Score);

        save.Save();
        Time.timeScale = 1f;
        ServiceLocator.Get<ISceneService>().Load(gameOverScene);
    }

    private void OnDestroy()
    {
        DeliveryManager.OnGameOver -= OnGameOver;
    }
}
