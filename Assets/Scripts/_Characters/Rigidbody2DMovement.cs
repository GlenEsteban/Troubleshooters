using UnityEngine;

// Handles movement and orientation using Rigidbody2D
public class Rigidbody2DMovement : MonoBehaviour {
    [Range(1f, 100f), SerializeField] float acceleration = 20f;
    [Range(1f, 100f), SerializeField] float deceleration = 5f;
    [Range(1f, 100f), SerializeField] float maxMoveSpeed = 8f;

    private bool canMove = true;

    private Rigidbody2D rb;

    private Vector2 moveDirection;

    public void SetCanMove(bool state) {
        canMove = state;
    }

    public Vector2 GetMoveDirection() {
        return moveDirection;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate() {
        if (!canMove) { return; }

        if (moveDirection != Vector2.zero && rb.linearVelocity.magnitude < maxMoveSpeed) {
            rb.linearVelocity += acceleration * moveDirection * Time.deltaTime;
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxMoveSpeed);
        }
        else {
            rb.linearVelocity -= deceleration * rb.linearVelocity * Time.deltaTime;
        }
    }
    public void MoveInDirection(Vector2 direction) {
        moveDirection = direction.normalized;
    }

    public void StopMovement() {
        moveDirection = Vector2.zero;
    }

    public void HardStopMovement() {
        rb.linearVelocity = Vector3.zero;
    }

    public void FlipHorizontalDirection() {
        moveDirection.x = -moveDirection.x;
    }

    public void MoveTowardPosition(Vector2 position) {
        moveDirection = position - rb.position;
    }

    public void SetLinearVelocity(Vector2 velocity) {
        rb.linearVelocity = velocity;
    }
}