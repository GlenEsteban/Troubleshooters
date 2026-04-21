using UnityEngine;

/// <summary>
/// Uses a move direction and Rigidbody2D for physics-based movement,
/// applying acceleration and deceleration for smooth motion.
/// 
/// Ground detection is used to ensure correct behavior for grounded characters.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Rigidbody2DMovement : MonoBehaviour {
    public Vector2 MoveDirection => moveDirection;

    [Header("Ground Detection")]
    [SerializeField] private Collider2D groundDetection;
    [SerializeField] private LayerMask groundLayers;

    [Header("Movement")]
    [Range(1f, 100f), SerializeField] float acceleration = 20f;
    [Range(1f, 100f), SerializeField] float deceleration = 12f;
    [Range(1f, 100f), SerializeField] float maxMoveSpeed = 10f;

    private Rigidbody2D rb;

    private bool canMove = true;
    private bool isGroundedCharacter;
    private bool isGrounded;

    private Vector2 moveDirection;

    private const float MIN_MOVE_THRESHOLD = 0.001f;

    public void SetCanMove(bool state) {
        canMove = state;
    }

    public void SetMoveDirection(Vector2 direction) {
        moveDirection = direction.normalized;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        isGroundedCharacter = rb.gravityScale != 0 ? true : false;
    }

    void FixedUpdate() {
        if (isGroundedCharacter) {
            if (groundDetection != null) {
                isGrounded = groundDetection.IsTouchingLayers(groundLayers);
            }
            else {
                Debug.LogError(gameObject.name + " Needs a ground detector for grounded rigidbody 2D movement!");
                return;
            }

            if (!isGrounded) { return; }
        }

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

    public void HardStopVelocity() {
        rb.linearVelocity = Vector2.zero;
    }

    public void StopMovement() {
        SetMoveDirection(Vector2.zero);
    }
}