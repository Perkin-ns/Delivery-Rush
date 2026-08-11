using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private float boostMultiplier = 1.5f;
    [SerializeField] private float boostDuration = 5f;
    [SerializeField] private float respawnDelay = 10f;

    private MeshRenderer meshRenderer;
    private Collider triggerCollider;
    private float respawnTimer;
    private bool isCollected;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        triggerCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!isCollected) return;

        respawnTimer -= Time.deltaTime;
        if (respawnTimer <= 0f)
        {
            isCollected = false;
            if (meshRenderer != null) meshRenderer.enabled = true;
            if (triggerCollider != null) triggerCollider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.TryGetComponent<PlayerMovement>(out var player))
        {
            player.ActivateBoost(boostMultiplier, boostDuration);

            isCollected = true;
            respawnTimer = respawnDelay;
            if (meshRenderer != null) meshRenderer.enabled = false;
            if (triggerCollider != null) triggerCollider.enabled = false;
        }
    }
}
