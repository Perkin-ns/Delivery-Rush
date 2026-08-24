using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0f, 3.5f, -7f);
    [SerializeField] private float topDownHeight = 15f;

    [Header("Smooth")]
    [SerializeField] private float positionSmoothTime = 0.3f;
    [SerializeField] private float rotationSmoothTime = 0.2f;

    private bool isTopDown;
    private Vector3 velocityRef;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            isTopDown = !isTopDown;
        }
    }

    private void FixedUpdate()
    {
        if (target == null && PlayerMovement.Instance != null)
            target = PlayerMovement.Instance.transform;
        if (target == null) return;
        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition;
        Quaternion targetRotation;

        if (isTopDown)
        {
            targetPosition = target.position + Vector3.up * topDownHeight;
            targetRotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            targetPosition = target.position
                + target.forward * thirdPersonOffset.z
                + target.up * thirdPersonOffset.y
                + target.right * thirdPersonOffset.x;

            Vector3 direction = (target.position - targetPosition).normalized;
            if (direction != Vector3.zero)
                targetRotation = Quaternion.LookRotation(direction);
            else
                targetRotation = transform.rotation;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocityRef, positionSmoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
    }
}
