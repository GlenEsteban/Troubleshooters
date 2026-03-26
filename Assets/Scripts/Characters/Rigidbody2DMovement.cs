using UnityEngine;

// Handles movement and orientation using Rigidbody2D
public class Rigidbody2DMovement : MonoBehaviour {
    [Range(1f, 100f), SerializeField] float _acceleration = 10f;
    [Range(1f, 100f), SerializeField] float _deceleration = 5f;
    [Range(1f, 100f), SerializeField] float _maxMoveSpeed = 8f;

    private Rigidbody2D _rb;

    private Vector2 _moveDirection;

    private bool _canMove = true;

    public void SetCanMove(bool state) {
        _canMove = state;
    }

    public Vector2 GetMoveDirection() {
        return _moveDirection;
    }

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate() {
        if (!_canMove) { return; }

        if (_moveDirection != Vector2.zero && _rb.linearVelocity.magnitude < _maxMoveSpeed) {
            _rb.linearVelocity += _acceleration * _moveDirection * Time.deltaTime;
            _rb.linearVelocity = Vector2.ClampMagnitude(_rb.linearVelocity, _maxMoveSpeed);
        }
        else {
            _rb.linearVelocity -= _deceleration * _rb.linearVelocity * Time.deltaTime;
        }
    }
    public void MoveInDirection(Vector2 direction) {
        _moveDirection = direction.normalized;
    }

    public void StopMovement() {
        _moveDirection = Vector2.zero;
    }

    public void HardStopMovement() {
        _rb.linearVelocity = Vector3.zero;
    }

    public void FlipHorizontalDirection() {
        _moveDirection.x = -_moveDirection.x;
    }

    public void MoveTowardPosition(Vector2 position) {
        _moveDirection = position - _rb.position;
    }

    public void SetLinearVelocity(Vector2 velocity) {
        _rb.linearVelocity = velocity;
    }
}