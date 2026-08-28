using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    private TMP_Text scoreText;
    private TMP_Text timerText;
    private TMP_Text instructionText;
    private TMP_Text boostText;
    private IDeliveryService delivery;

    private void Awake()
    {
        delivery = ServiceLocator.Get<IDeliveryService>();

        IUIFactory ui = ServiceLocator.Get<IUIFactory>();
        ui.EnsureCanvas(gameObject);
        TMP_FontAsset font = ui.LoadFont();

        scoreText = ui.CreateText(transform, "ScoreText", new Vector2(-400f, -20f), new Vector2(300f, 50f), 28, font);
        timerText = ui.CreateText(transform, "TimerText", new Vector2(400f, -20f), new Vector2(300f, 50f), 28, font);
        instructionText = ui.CreateText(transform, "InstructionText", new Vector2(0f, 60f), new Vector2(300f, 50f), 24, font);
        boostText = ui.CreateText(transform, "BoostText", new Vector2(0f, 0f), new Vector2(300f, 50f), 22, font);
        boostText.color = Color.yellow;
        boostText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (delivery == null) return;

        scoreText.text = $"Score: {delivery.Score}";

        float time = delivery.TimeRemaining;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";

        if (delivery.HasActiveDelivery)
            instructionText.text = $"Deliver to: {delivery.CurrentDeliveryName}";
        else
            instructionText.text = $"Pick up: {delivery.CurrentPickupName}";

        if (boostText == null) return;
        if (!ServiceLocator.TryGet<IPlayerService>(out var player)) return;

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
