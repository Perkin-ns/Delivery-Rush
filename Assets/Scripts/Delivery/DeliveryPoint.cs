using UnityEngine;

public class DeliveryPoint : TriggerMarker
{
    public string displayName = "Delivery";

    protected override void InitializeColors()
    {
        activeColor = Color.blue;
        inactiveColor = Color.gray;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActive) return;
        if (!ServiceLocator.TryGet<IDeliveryService>(out var delivery)) return;
        if (!other.TryGetComponent<IPlayerService>(out _)) return;

        if (delivery.TryCompleteDelivery(this))
        {
            SetVisualActive(false);
        }
    }
}
