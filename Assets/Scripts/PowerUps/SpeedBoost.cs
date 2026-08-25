using System.Collections;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private float boostMultiplier = 1.5f;
    [SerializeField] private float boostDuration = 5f;
    [SerializeField] private float respawnDelay = 10f;

    private MeshRenderer meshRenderer;
    private Collider triggerCollider;
    private Coroutine respawnCoroutine;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerMovement>(out var player)) return;

        player.ActivateBoost(boostMultiplier, boostDuration);

        if (meshRenderer != null) meshRenderer.enabled = false;
        if (triggerCollider != null) triggerCollider.enabled = false;

        if (respawnCoroutine != null)
            StopCoroutine(respawnCoroutine);
        respawnCoroutine = StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (meshRenderer != null) meshRenderer.enabled = true;
        if (triggerCollider != null) triggerCollider.enabled = true;
        respawnCoroutine = null;
    }
}
