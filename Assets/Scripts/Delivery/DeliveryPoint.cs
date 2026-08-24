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
        if (DeliveryManager.Instance == null) return;
        if (!other.TryGetComponent<PlayerMovement>(out _)) return;

        if (DeliveryManager.Instance.TryCompleteDelivery(this))
        {
            SetVisualActive(false);
        }
    }
}
