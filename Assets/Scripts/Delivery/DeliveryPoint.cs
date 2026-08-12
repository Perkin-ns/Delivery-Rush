using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    public string displayName = "Delivery";

    [Header("Visuals")]
    [SerializeField] private Color activeColor = Color.blue;
    [SerializeField] private Color inactiveColor = Color.gray;

    private MeshRenderer meshRenderer;
    private Collider triggerCollider;
    private bool isActive;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (DeliveryManager.Instance == null) return;
        if (!other.TryGetComponent<PlayerMovement>(out _)) return;

        if (DeliveryManager.Instance.TryCompleteDelivery(this))
        {
            SetVisualActive(false);
        }
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
