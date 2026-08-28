using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IPlayerService
{
    [Header("Movement Settings")]
    [SerializeField] private float turnSpeed = 30f;
    [SerializeField] private float driftFactor = 0.95f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float maxAcceleration = 15f;
    [SerializeField] private float reverseMaxSpeed = 12f;
    [SerializeField] private float steeringFalloff = 0.6f;

    [Header("Brake Settings")]
    [SerializeField] private float brakeForce = 15f;
    [SerializeField] private float brakeThreshold = 2f;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private bool isBraking;

    private bool isBoosted;
    private float boostTimer;
    private float originalMaxSpeed;

    public bool IsBoosted => isBoosted;
    public float BoostTimeRemaining => isBoosted ? boostTimer : 0f;
    public Transform Transform => transform;

    private void Awake()
    {
        ServiceLocator.Register<IPlayerService>(this);
        rb = GetComponent<Rigidbody>();
        originalMaxSpeed = maxSpeed;
    }

    private void Update()
    {
        ReadInput();

        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosted = false;
                maxSpeed = originalMaxSpeed;
            }
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ReadInput()
    {
        if (Keyboard.current == null) return;

        moveInput = 0f;
        isBraking = false;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            moveInput += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
            if (forwardSpeed > brakeThreshold)
            {
                isBraking = true;
            }
            else
            {
                moveInput -= 1f;
            }
        }

        turnInput = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            turnInput -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            turnInput += 1f;
    }

    private void ApplyMovement()
    {
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (isBraking && forwardSpeed > 0.1f)
        {
            rb.AddForce(-transform.forward * brakeForce, ForceMode.Acceleration);
        }

        if (moveInput != 0f)
        {
            float targetSpeed = moveInput > 0f ? maxSpeed : -reverseMaxSpeed;
            float speedDiff = targetSpeed - forwardSpeed;
            float moveForce = Mathf.Clamp(speedDiff * acceleration, -maxAcceleration, maxAcceleration);
            rb.AddForce(transform.forward * moveForce, ForceMode.Acceleration);
        }

        if (moveInput != 0f)
        {
            float speedRatio = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);
            float steerScale = 1f - speedRatio * steeringFalloff;
            rb.AddTorque(transform.up * turnInput * turnSpeed * steerScale * Mathf.Sign(moveInput), ForceMode.Acceleration);
        }

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        float horizontalForwardSpeed = Vector3.Dot(horizontalVelocity, transform.forward);
        float sidewaysSpeed = Vector3.Dot(horizontalVelocity, transform.right);

        rb.linearVelocity = transform.forward * horizontalForwardSpeed
                          + transform.right * sidewaysSpeed * driftFactor
                          + Vector3.up * rb.linearVelocity.y;
    }

    public void ActivateBoost(float multiplier, float duration)
    {
        if (!isBoosted)
            originalMaxSpeed = maxSpeed;

        isBoosted = true;
        boostTimer = duration;
        maxSpeed = originalMaxSpeed * multiplier;
    }
}
