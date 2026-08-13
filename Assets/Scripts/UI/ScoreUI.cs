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

        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        if (GetComponent<UnityEngine.UI.CanvasScaler>() == null)
            gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        if (GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        scoreText = CreateText("ScoreText", new Vector2(-400f, -20f), 28, font);
        timerText = CreateText("TimerText", new Vector2(400f, -20f), 28, font);
        instructionText = CreateText("InstructionText", new Vector2(0f, 60f), 24, font);
        boostText = CreateText("BoostText", new Vector2(0f, 0f), 22, font);
        boostText.color = Color.yellow;
        boostText.gameObject.SetActive(false);
    }

    private TMP_Text CreateText(string name, Vector2 position, int fontSize, TMP_FontAsset font)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(300f, 50f);

        TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.font = font;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        return tmp;
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
