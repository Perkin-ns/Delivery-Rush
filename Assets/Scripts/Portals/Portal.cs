using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Portal linkedPortal;
    [SerializeField] private float cooldown = 1.5f;
    [SerializeField] private float exitOffset = 3f;

    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cooldownTimer > 0f) return;
        if (linkedPortal == null) return;
        if (!other.TryGetComponent<PlayerMovement>(out _)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        Vector3 exitDir = linkedPortal.transform.forward;
        exitDir.y = 0f;
        if (exitDir.sqrMagnitude < 0.001f)
            exitDir = Vector3.forward;
        exitDir.Normalize();

        Vector3 exitPosition = linkedPortal.transform.position + exitDir * exitOffset;
        exitPosition.y = linkedPortal.transform.position.y;

        rb.position = exitPosition;

        cooldownTimer = cooldown;
        linkedPortal.cooldownTimer = cooldown;
    }
}
