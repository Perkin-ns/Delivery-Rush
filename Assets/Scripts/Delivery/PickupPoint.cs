using UnityEngine;

public class PickupPoint : TriggerMarker
{
    public static readonly System.Collections.Generic.List<PickupPoint> All = new();

    [SerializeField] private DeliveryPoint pairedDeliveryPoint;
    [SerializeField] private string pickupName = "Package";

    public string PickupName => pickupName;

    protected override void InitializeColors()
    {
        activeColor = Color.green;
        inactiveColor = Color.gray;
    }

    protected override void OnAwake()
    {
        All.Add(this);
    }

    private void OnDestroy()
    {
        All.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActive) return;
        if (!ServiceLocator.TryGet<IDeliveryService>(out var delivery)) return;
        if (delivery.HasActiveDelivery) return;
        if (!other.TryGetComponent<IPlayerService>(out _)) return;

        delivery.StartDelivery(pairedDeliveryPoint, pickupName);
        SetVisualActive(false);
        if (pairedDeliveryPoint != null)
            pairedDeliveryPoint.SetVisualActive(true);
    }
}
