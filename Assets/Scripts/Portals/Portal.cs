using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Portal linkedPortal;
    [SerializeField] private float cooldown = 1.5f;
    [SerializeField] private float exitOffset = 3f;

    private bool isOnCooldown;
    private Coroutine cooldownCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (isOnCooldown) return;
        if (linkedPortal == null) return;
        if (!other.TryGetComponent<IPlayerService>(out _)) return;

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

        EnterCooldown();
        linkedPortal.EnterCooldown();
    }

    private void EnterCooldown()
    {
        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);
        isOnCooldown = true;
        cooldownCoroutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
        cooldownCoroutine = null;
    }
}
