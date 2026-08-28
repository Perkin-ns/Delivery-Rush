using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    private void Awake()
    {
        IUIFactory ui = ServiceLocator.Get<IUIFactory>();
        ui.EnsureCanvas(gameObject);
        TMP_FontAsset font = ui.LoadFont();

        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.transform.SetParent(transform, false);
        eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        ui.CreateText(transform, "TitleText", new Vector2(0f, 140f), new Vector2(500f, 60f), 48, font).text = "GAME OVER";

        IPersistenceService save = ServiceLocator.Get<IPersistenceService>();
        int score = save.GetInt("FinalScore", 0);
        int deliveries = save.GetInt("DeliveriesCompleted", 0);

        ui.CreateText(transform, "ScoreText", new Vector2(0f, 40f), new Vector2(500f, 60f), 32, font).text = $"Score: {score}";
        ui.CreateText(transform, "DeliveriesText", new Vector2(0f, -20f), new Vector2(500f, 60f), 28, font).text = $"Deliveries Completed: {deliveries}";

        TMP_Text leaderboardText = ui.CreateText(transform, "LeaderboardText", Vector2.zero, new Vector2(400f, 260f), 20, font);
        RectTransform lbRect = leaderboardText.rectTransform;
        lbRect.anchorMin = new Vector2(0f, 1f);
        lbRect.anchorMax = new Vector2(0f, 1f);
        lbRect.pivot = new Vector2(0f, 1f);
        lbRect.anchoredPosition = new Vector2(20f, -20f);
        leaderboardText.alignment = TextAlignmentOptions.TopLeft;
        leaderboardText.text = BuildLeaderboard();

        CreateButton("PlayAgainButton", "Play Again", new Vector2(0f, -120f), font, () => ServiceLocator.Get<ISceneService>().Load("Game"));
        CreateButton("MainMenuButton", "Main Menu", new Vector2(0f, -200f), font, () => ServiceLocator.Get<ISceneService>().Load("MainMenu"));
    }

    private string BuildLeaderboard()
    {
        var top = ServiceLocator.Get<ILeaderboardService>().GetTop(10);
        var sb = new System.Text.StringBuilder("LEADERBOARD");
        for (int i = 0; i < top.Count; i++)
            sb.Append($"\n{i + 1}. {top[i].name}  {top[i].score}");
        return sb.ToString();
    }

    private void CreateButton(string name, string label, Vector2 position, TMP_FontAsset font, System.Action onClick)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(220f, 60f);

        UnityEngine.UI.Image image = go.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0.2f, 0.6f, 1f, 1f);

        UnityEngine.UI.Button button = go.AddComponent<UnityEngine.UI.Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => onClick());

        GameObject labelGO = new GameObject("Text");
        labelGO.transform.SetParent(go.transform, false);

        RectTransform labelRect = labelGO.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.sizeDelta = Vector2.zero;

        TMP_Text labelText = labelGO.AddComponent<TextMeshProUGUI>();
        labelText.font = font;
        labelText.fontSize = 28;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.text = label;
    }
}
