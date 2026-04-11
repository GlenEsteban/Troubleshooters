using UnityEngine;

/// <summary>
/// Implements grounded follow behavior that moves the character along the ground
/// toward a target with optional edge/obstacle detection.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(Rigidbody2DMovement))]
public class GroundedAIBehaviorFollowTarget : GroundedAIBehavior {
    [Header("Target Following")]
    [SerializeField] private Transform target;
    [SerializeField, Range(0.01f, 1f)] private float followingInterval = 0.1f;
    [SerializeField] private bool useHardStopAtTarget = true;
    [Range(0.2f, 10f), SerializeField] private float stoppingDistance = 1f;

    [Header("Detection Timing")]
    [SerializeField, Range(0.01f, 1f)] private float detectionInterval = 0.1f;

    [Header("Edge Detection")]
    [SerializeField] private bool useEdgeDetection = true;
    [SerializeField] private Collider2D edgeDetection;

    [Header("Obstacle Detection")]
    [SerializeField] private bool useObstacleDetection = true;
    [SerializeField] private Collider2D obstacleDetection;
    [SerializeField] private LayerMask obstacleLayers;

    private LookOrientation lookOrientation;
    private Rigidbody2DMovement rigidBody2DMovement;

    private float followingTargetTimer;
    private float detectionTimer;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
    }

    public override void UpdateBehavior() {
        if (CheckIfGrounded()) {
            followingTargetTimer += Time.deltaTime;

            if (followingTargetTimer > followingInterval) {
                UpdateMoveDirection();

                followingTargetTimer = 0;
            }

            detectionTimer += Time.deltaTime;

            if (detectionTimer > detectionInterval) {
                HardStopAtEdge();
                HardStopAtObstacle();

                detectionTimer = 0;
            }
        }
    }

    private void UpdateMoveDirection() {
        if (target == null) { return; }

        float signedHorizontalDistanceToTarget = target.position.x - transform.position.x;
        float horizontalDistanceToTarget = Mathf.Abs(signedHorizontalDistanceToTarget);

        if (horizontalDistanceToTarget > stoppingDistance) {
            Vector2 moveDirection = signedHorizontalDistanceToTarget > 0 ? Vector2.right : Vector2.left;

            rigidBody2DMovement.SetMoveDirection(moveDirection);
            lookOrientation.SetLookDirection(moveDirection);
        }

        if (useHardStopAtTarget && horizontalDistanceToTarget < stoppingDistance) {
            rigidBody2DMovement.SetMoveDirection(Vector2.zero);
        }
    }

    private void HardStopAtEdge() {
        if (!useEdgeDetection || edgeDetection == null) { return; }

        if (!edgeDetection.IsTouchingLayers(groundLayers)) {
            if (rigidBody2DMovement.GetMoveDirection() == Vector2.zero) { return; }

            rigidBody2DMovement.HardStopMovement();
            rigidBody2DMovement.SetMoveDirection(Vector2.zero);
        }
    }

    private void HardStopAtObstacle() {
        if (!useObstacleDetection || obstacleDetection == null) { return; }

        if (obstacleDetection.IsTouchingLayers(obstacleLayers)) {
            if (rigidBody2DMovement.GetMoveDirection() == Vector2.zero) { return; }

            rigidBody2DMovement.HardStopMovement();
            rigidBody2DMovement.SetMoveDirection(Vector2.zero);
        }
    }
}