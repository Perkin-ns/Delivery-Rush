using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    [Header("Scoring")]
    [SerializeField] private int baseScore = 50;
    [SerializeField] private int maxBonus = 50;
    [SerializeField] private float maxDeliveryTime = 30f;

    [Header("Timer")]
    [SerializeField] private float gameDuration = 120f;

    public int Score { get; private set; }
    public int DeliveriesCompleted { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool HasActiveDelivery { get; private set; }
    public string CurrentDeliveryName => HasActiveDelivery && currentTarget != null ? currentTarget.displayName : "";
    public string CurrentPickupName => !HasActiveDelivery && pickupPoints != null && currentPickupIndex < pickupPoints.Length
        ? pickupPoints[currentPickupIndex].PickupName : "";

    public PickupPoint[] pickupPoints;
    private int currentPickupIndex = -1;
    private float deliveryStartTime;
    private DeliveryPoint currentTarget;

    public System.Action<int> OnScoreChanged;
    public System.Action<bool> OnDeliveryStateChanged;
    public System.Action<string> OnInstructionChanged;
    public System.Action OnGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        TimeRemaining = gameDuration;
    }

    private void Start()
    {
        if (pickupPoints == null || pickupPoints.Length == 0)
            pickupPoints = FindObjectsOfType<PickupPoint>();

        if (pickupPoints != null && pickupPoints.Length > 0)
        {
            for (int i = 0; i < pickupPoints.Length; i++)
                pickupPoints[i].SetVisualActive(false);

            currentPickupIndex = 0;
            pickupPoints[currentPickupIndex].SetVisualActive(true);
            OnInstructionChanged?.Invoke("Pick up: " + pickupPoints[currentPickupIndex].PickupName);
        }
    }

    private void Update()
    {
        if (IsGameOver) return;

        TimeRemaining -= Time.deltaTime;
        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            IsGameOver = true;
            OnGameOver?.Invoke();
        }
    }

    public void StartDelivery(DeliveryPoint target, string pickupName)
    {
        if (HasActiveDelivery || IsGameOver) return;

        HasActiveDelivery = true;
        currentTarget = target;
        deliveryStartTime = Time.time;
        OnDeliveryStateChanged?.Invoke(true);
        OnInstructionChanged?.Invoke("Deliver to: " + target.displayName);
    }

    public bool TryCompleteDelivery(DeliveryPoint point)
    {
        if (!HasActiveDelivery || IsGameOver) return false;
        if (point != currentTarget) return false;

        float elapsed = Time.time - deliveryStartTime;
        int bonus = Mathf.RoundToInt(Mathf.Max(0, maxBonus * (1f - elapsed / maxDeliveryTime)));
        int points = baseScore + bonus;

        Score += points;
        DeliveriesCompleted++;
        HasActiveDelivery = false;
        currentTarget = null;

        OnScoreChanged?.Invoke(Score);
        OnDeliveryStateChanged?.Invoke(false);

        if (pickupPoints != null && pickupPoints.Length > 0)
        {
            currentPickupIndex = (currentPickupIndex + 1) % pickupPoints.Length;
            pickupPoints[currentPickupIndex].SetVisualActive(true);
            OnInstructionChanged?.Invoke("Pick up: " + pickupPoints[currentPickupIndex].PickupName);
        }
        else
        {
            OnInstructionChanged?.Invoke("Pick up a package!");
        }

        return true;
    }

    public void CancelDelivery()
    {
        if (!HasActiveDelivery) return;

        HasActiveDelivery = false;
        currentTarget = null;
        OnDeliveryStateChanged?.Invoke(false);
        OnInstructionChanged?.Invoke("Pick up a package!");
    }
}
