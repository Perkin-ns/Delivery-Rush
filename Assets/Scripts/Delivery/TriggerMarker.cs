using UnityEngine;

public abstract class TriggerMarker : MonoBehaviour
{
    protected Color activeColor;
    protected Color inactiveColor;

    private MeshRenderer meshRenderer;
    private Collider triggerCollider;
    private bool isActive;

    protected bool IsActive => isActive;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        triggerCollider = GetComponent<Collider>();
        InitializeColors();
        OnAwake();
    }

    protected abstract void InitializeColors();
    protected virtual void OnAwake() { }

    public void SetVisualActive(bool active)
    {
        isActive = active;
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        if (triggerCollider == null) triggerCollider = GetComponent<Collider>();
        if (meshRenderer != null)
            meshRenderer.material.color = active ? activeColor : inactiveColor;
        if (triggerCollider != null)
            triggerCollider.enabled = active;
    }
}
