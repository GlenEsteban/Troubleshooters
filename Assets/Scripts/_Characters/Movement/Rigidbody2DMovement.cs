using UnityEngine;

/// <summary>
/// Uses a move direction and Rigidbody2D for physics-based movement,
/// applying acceleration and deceleration for smooth motion.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Rigidbody2DMovement : MonoBehaviour {
    [Header("Movement")]
    [Range(1f, 100f), SerializeField] float acceleration = 20f;
    [Range(1f, 100f), SerializeField] float deceleration = 12f;
    [Range(1f, 100f), SerializeField] float maxMoveSpeed = 10f;

    private Rigidbody2D rb;

    private bool canMove = true;

    private Vector2 moveDirection;

    private const float MIN_MOVE_THRESHOLD = 0.0001f;

    public void SetCanMove(bool state) {
        canMove = state;
    }
    public void SetMoveDirection(Vector2 direction) {
        moveDirection = direction.normalized;
    }

    public Vector2 GetMoveDirection() {
        return moveDirection;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (!canMove || moveDirection.sqrMagnitude <= MIN_MOVE_THRESHOLD) {
            ApplyMovementDeceleration();
        }
        else {
            ApplyMovementAcceleration();
        }
    }

    private void ApplyMovementDeceleration() {
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
    }

    private void ApplyMovementAcceleration() {
        Vector2 targetVelocity = maxMoveSpeed * moveDirection;

        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    public void HardStopMovement() {
        rb.linearVelocity = Vector2.zero;
    }
}