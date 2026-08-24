using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    private TMP_Text scoreText;
    private TMP_Text timerText;
    private TMP_Text instructionText;
    private TMP_Text boostText;

    private void Awake()
    {
        if (DeliveryManager.Instance == null)
            gameObject.AddComponent<DeliveryManager>();

        UIFactory.EnsureCanvas(gameObject);
        TMP_FontAsset font = UIFactory.LoadFont();

        scoreText = UIFactory.CreateText(transform, "ScoreText", new Vector2(-400f, -20f), new Vector2(300f, 50f), 28, font);
        timerText = UIFactory.CreateText(transform, "TimerText", new Vector2(400f, -20f), new Vector2(300f, 50f), 28, font);
        instructionText = UIFactory.CreateText(transform, "InstructionText", new Vector2(0f, 60f), new Vector2(300f, 50f), 24, font);
        boostText = UIFactory.CreateText(transform, "BoostText", new Vector2(0f, 0f), new Vector2(300f, 50f), 22, font);
        boostText.color = Color.yellow;
        boostText.gameObject.SetActive(false);
    }

    private void Update()
    {
        DeliveryManager dm = DeliveryManager.Instance;
        if (dm == null) return;

        scoreText.text = $"Score: {dm.Score}";

        float time = dm.TimeRemaining;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";

        if (dm.HasActiveDelivery)
            instructionText.text = $"Deliver to: {dm.CurrentDeliveryName}";
        else
            instructionText.text = $"Pick up: {dm.CurrentPickupName}";

        PlayerMovement player = PlayerMovement.Instance;

        if (player != null && boostText != null)
        {
            if (player.IsBoosted)
            {
                boostText.gameObject.SetActive(true);
                boostText.text = $"SPEED BOOST: {player.BoostTimeRemaining:F1}s";
            }
            else
            {
                boostText.gameObject.SetActive(false);
            }
        }
    }
}
