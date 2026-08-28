using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IDeliveryService
{
    int Score { get; }
    int DeliveriesCompleted { get; }
    float TimeRemaining { get; }
    bool IsGameOver { get; }
    bool HasActiveDelivery { get; }
    string CurrentPickupName { get; }
    string CurrentDeliveryName { get; }
    bool TryGetCurrentTarget(out Vector3 position);
    void StartDelivery(DeliveryPoint target, string pickupName);
    bool TryCompleteDelivery(DeliveryPoint point);
}

public interface IPlayerService
{
    Transform Transform { get; }
    bool IsBoosted { get; }
    float BoostTimeRemaining { get; }
    void ActivateBoost(float multiplier, float duration);
}

public interface ISceneService
{
    void Load(string sceneName);
}

public interface IPersistenceService
{
    int GetInt(string key, int defaultValue);
    void SetInt(string key, int value);
    string GetString(string key, string defaultValue);
    void SetString(string key, string value);
    void Save();
}

public interface ILeaderboardService
{
    void Add(string name, int score);
    System.Collections.Generic.IReadOnlyList<LeaderboardEntry> GetTop(int count);
}

public interface IUIFactory
{
    TMP_FontAsset LoadFont();
    Canvas EnsureCanvas(GameObject go);
    TMP_Text CreateText(Transform parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize, TMP_FontAsset font);
}
