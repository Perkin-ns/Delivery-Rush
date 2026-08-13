using UnityEngine;

public class PickupPoint : MonoBehaviour
{
    public static readonly System.Collections.Generic.List<PickupPoint> All = new();

    [SerializeField] private DeliveryPoint pairedDeliveryPoint;
    [SerializeField] private string pickupName = "Package";

    [Header("Visuals")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;

    private MeshRenderer meshRenderer;
    private Collider triggerCollider;
    private bool isActive;

    public string PickupName => pickupName;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        triggerCollider = GetComponent<Collider>();
        All.Add(this);
    }

    private void OnDestroy()
    {
        All.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (DeliveryManager.Instance == null) return;
        if (DeliveryManager.Instance.HasActiveDelivery) return;
        if (!other.TryGetComponent<PlayerMovement>(out _)) return;

        DeliveryManager.Instance.StartDelivery(pairedDeliveryPoint, pickupName);
        SetVisualActive(false);
        if (pairedDeliveryPoint != null)
            pairedDeliveryPoint.SetVisualActive(true);
    }

    public void SetVisualActive(bool active)
    {
        isActive = active;
        if (meshRenderer != null)
            meshRenderer.material.color = active ? activeColor : inactiveColor;
        if (triggerCollider != null)
            triggerCollider.enabled = active;
    }
}
