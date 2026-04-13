using UnityEngine;

/// <summary>
/// Follows a target along the ground and optionally uses edge and obstacle detection
/// to restrict movement.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(Rigidbody2DMovement))]
public class GroundedAIBehaviorFollowTarget : AIBehavior {
    public override bool RequiresLookOrientation => true;
    public override bool RequiresRigidbody2DMovement => true;

    [Header("Target Following")]
    [SerializeField] private Transform target;
    [SerializeField] private bool useHardStopAtTarget = true;
    [Range(0.2f, 10f), SerializeField] private float stoppingDistance = 1f;
    [SerializeField, Range(0.01f, 1f)] private float followingInterval = 0.1f;

    [Header("Detection Timing")]
    [SerializeField, Range(0.01f, 1f)] private float detectionInterval = 0.1f;

    [Header("Ground Detection")]
    [SerializeField] private Collider2D groundDetection;
    [SerializeField] private LayerMask groundLayers;

    [Header("Edge Detection")]
    [SerializeField] private bool useEdgeDetection = true;
    [SerializeField] private Collider2D edgeDetection;
    [SerializeField] private bool useHardStopAtEdge = true;

    [Header("Obstacle Detection")]
    [SerializeField] private bool useObstacleDetection = true;
    [SerializeField] private Collider2D obstacleDetection;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private bool useHardStopAtObstacle = true;

    private LookOrientation lookOrientation;
    private Rigidbody2DMovement rigidBody2DMovement;

    private float signedHorizontalDistanceToTarget;
    private float horizontalDistanceToTarget;

    private float followingTargetTimer;
    private float detectionTimer;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
    }

    public override void UpdateBehavior() {
        if (!CheckIfGrounded() || target == null) {
            StopMovement();
            return;
        }

        followingTargetTimer += Time.deltaTime;

        if (followingTargetTimer >= followingInterval) {
            UpdateHorizontalDistanceToTargetXPosition();

            if (horizontalDistanceToTarget > stoppingDistance) {
                MoveAndOrientToTargetXPosition();
            }
            else {
                StopAtTargetXPosition();
            }

            followingTargetTimer = 0f;
        }

        detectionTimer += Time.deltaTime;

        if (detectionTimer >= detectionInterval) {
            StopAtEdge();
            StopAtObstacle();

            detectionTimer = 0f;
        }
    }

    private bool CheckIfGrounded() {
        if (groundDetection == null) { return false; }

        return groundDetection.IsTouchingLayers(groundLayers);
    }

    private void UpdateHorizontalDistanceToTargetXPosition() {
        signedHorizontalDistanceToTarget = target.position.x - transform.position.x;
        horizontalDistanceToTarget = Mathf.Abs(signedHorizontalDistanceToTarget);
    }

    private void MoveAndOrientToTargetXPosition() {
        Vector2 moveDirection = signedHorizontalDistanceToTarget >= 0 ? Vector2.right : Vector2.left;

        rigidBody2DMovement.SetMoveDirection(moveDirection);
        lookOrientation.SetLookDirection(moveDirection);
    }

    private void StopAtTargetXPosition() {
        if (useHardStopAtTarget) {
            rigidBody2DMovement.HardStopMovement();
        }

        StopMovement();
    }

    private void StopAtEdge() {
        if (!useEdgeDetection || edgeDetection == null) { return; }
        if (rigidBody2DMovement.GetMoveDirection() == Vector2.zero) { return; }

        if (!edgeDetection.IsTouchingLayers(groundLayers)) {
            if (useHardStopAtEdge) {
                rigidBody2DMovement.HardStopMovement();
            }

            StopMovement();
        }
    }

    private void StopAtObstacle() {
        if (!useObstacleDetection || obstacleDetection == null) { return; }
        if (rigidBody2DMovement.GetMoveDirection() == Vector2.zero) { return; }

        if (obstacleDetection.IsTouchingLayers(obstacleLayers)) {
            if (useHardStopAtObstacle) {
                rigidBody2DMovement.HardStopMovement();
            }

            StopMovement();
        }
    }

    private void StopMovement() {
        rigidBody2DMovement.SetMoveDirection(Vector2.zero);
    }
}