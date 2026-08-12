using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float driftFactor = 0.95f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float maxSpeed = 30f;

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

    private void Awake()
    {
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

        if (isBraking)
        {
            if (forwardSpeed > 0.1f)
            {
                rb.AddForce(-transform.forward * brakeForce, ForceMode.Acceleration);
            }
        }

        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - forwardSpeed;
        float accelRate = Mathf.Abs(targetSpeed) > 0.1f ? acceleration : acceleration * 2f;
        float moveForce = speedDiff * accelRate;

        rb.AddForce(transform.forward * moveForce, ForceMode.Acceleration);

        if (moveInput != 0f)
        {
            rb.AddTorque(transform.up * turnInput * turnSpeed * Mathf.Sign(moveInput), ForceMode.Acceleration);
        }

        Vector3 forwardVelocity = Vector3.Dot(rb.linearVelocity, transform.forward) * transform.forward;
        Vector3 sidewaysVelocity = Vector3.Dot(rb.linearVelocity, transform.right) * transform.right;
        rb.linearVelocity = forwardVelocity + sidewaysVelocity * driftFactor;
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
