using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Uses a move direction, NavMeshAgent for pathfinding, and Rigidbody2D
/// for physics-based movement, applying acceleration and deceleration 
/// for smooth motion.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody2D))]
public class NavMeshRigidbody2DMovement : MonoBehaviour {
    public bool CanMove => canMove;

    [SerializeField, Range(1f, 100f)] private float acceleration = 20f;
    [SerializeField, Range(1f, 100f)] private float deceleration = 12f;
    [SerializeField, Range(1f, 100f)] private float maxMoveSpeed = 10f;
    [SerializeField] private float stoppingDistance = 1.5f;

    private NavMeshAgent navMeshAgent;
    private Rigidbody2D rb;

    private bool canMove = true;

    public void SetCanMove(bool state) {
        canMove = state;

        if (!canMove) {
            StopNavMeshNavigation();
        }
    }

    public Vector2 GetMoveDirection() {
        return navMeshAgent.desiredVelocity.normalized;
    }

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.stoppingDistance = stoppingDistance;
    }

    private void OnEnable() {
        if (!navMeshAgent.isOnNavMesh) { return; }

        navMeshAgent.Warp(transform.position);
    }

    private void Update() {
        if (!navMeshAgent.isOnNavMesh) { return; }

        navMeshAgent.nextPosition = rb.position;
    }

    private void FixedUpdate() {
        if (!navMeshAgent.isOnNavMesh) { return; }

        if (!canMove || !navMeshAgent.hasPath || navMeshAgent.pathPending ||
            navMeshAgent.remainingDistance <= stoppingDistance) {
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
        Vector2 targetVelocity = Vector2.ClampMagnitude(navMeshAgent.desiredVelocity, maxMoveSpeed);

        rb.linearVelocity = Vector2.MoveTowards( rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    public void MoveToTarget(Vector2 targetPosition) {
        if (!navMeshAgent.isOnNavMesh) { return; }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
    }

    public void StopNavMeshNavigation() {
        if (!navMeshAgent.isOnNavMesh) { return; }

        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
    }
}