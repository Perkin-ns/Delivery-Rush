using UnityEngine;

public class NPCReceiver : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Color receiveColor = Color.green;
    [SerializeField] private float receiveDuration = 2f;

    private MeshRenderer meshRenderer;
    private Color originalColor;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            originalColor = meshRenderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DeliveryManager.Instance == null) return;
        if (!other.CompareTag("Player")) return;
        if (!DeliveryManager.Instance.HasActiveDelivery) return;

        DeliveryManager.Instance.TryCompleteDelivery(GetComponent<DeliveryPoint>());
    }

    public void ShowReceive()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = receiveColor;
            Invoke(nameof(ResetColor), receiveDuration);
        }
    }

    private void ResetColor()
    {
        if (meshRenderer != null)
            meshRenderer.material.color = originalColor;
    }
}
