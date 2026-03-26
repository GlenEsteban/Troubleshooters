using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody2D))]
public class NavMeshRigidbody2DMovement : MonoBehaviour {
    [Range(1f, 100f), SerializeField] float _acceleration = 60f;
    [Range(1f, 100f), SerializeField] float _deceleration = 5f;
    [Range(1f, 100f), SerializeField] float _maxMoveSpeed = 8f;
    [SerializeField] private float _stoppingDistance = 1.5f;

    private NavMeshAgent _navMeshAgent;
    private Rigidbody2D _rb;

    private bool _canMove = true;

    public void SetCanMove(bool state) {
        _canMove = state;
    }

    public Vector2 GetCurrentMoveDirection() {
        return _navMeshAgent.desiredVelocity.normalized;
    }

    public void MoveToTarget(Vector2 targetPosition) {
        if (!_navMeshAgent.isOnNavMesh) { return; }

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(targetPosition);
    }

    public void StopMovement() {
        _navMeshAgent.isStopped = true;
    }

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody2D>();

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
    }

    private void OnEnable() {
        _navMeshAgent.Warp(transform.position);
    }

    private void Update() {
        if (!_navMeshAgent.isOnNavMesh) { return; }

        _navMeshAgent.nextPosition = _rb.position;
    }

    private void FixedUpdate() {
        if (!_canMove) { return; }

        Vector2 desiredDirection = _navMeshAgent.desiredVelocity.normalized;

        if (!_navMeshAgent.isOnNavMesh || _navMeshAgent.pathPending || !_navMeshAgent.hasPath || 
            _navMeshAgent.remainingDistance <= _stoppingDistance) {
            _rb.linearVelocity -= _deceleration * _rb.linearVelocity * Time.deltaTime;
        }
        else if (desiredDirection != Vector2.zero && _rb.linearVelocity.magnitude < _maxMoveSpeed) {
            _rb.linearVelocity += _acceleration * desiredDirection * Time.deltaTime;
            _rb.linearVelocity = Vector2.ClampMagnitude(_rb.linearVelocity, _maxMoveSpeed);
        }
    }
}