using UnityEngine;

/// <summary>
/// Implements grounded patrol behavior that moves the character along the ground
/// and reverses direction when edges or obstacles are detected.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(Rigidbody2DMovement))]
public class GroundedAIBehaviorSimplePatrol : GroundedAIBehavior {
    [Header("Movement")]
    [SerializeField] private Vector2 startingMoveDirection = Vector2.right;
    [SerializeField] private bool useHardStopOnTurnAround = true;

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

    private float detectionTimer;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
    }

    private void Start() {
        startingMoveDirection = startingMoveDirection.normalized;

        rigidBody2DMovement.SetMoveDirection(startingMoveDirection);
        lookOrientation.SetLookDirection(startingMoveDirection);
    }

    public override void UpdateBehavior() {
        if (CheckIfGrounded()) {
            detectionTimer += Time.deltaTime;

            if (detectionTimer > detectionInterval) {
                TurnAroundAtEdge();
                TurnAroundAtObstacle();

                detectionTimer = 0;
            }
        }
    }

    private void TurnAroundAtEdge() {
        if (!useEdgeDetection || edgeDetection == null) { return; }

        if (!edgeDetection.IsTouchingLayers(groundLayers)) {
            if (useHardStopOnTurnAround) {
                rigidBody2DMovement.HardStopMovement();
            }

            TurnAround();
        }
    }

    private void TurnAroundAtObstacle() {
        if (!useObstacleDetection || obstacleDetection == null) { return; }

        if (obstacleDetection.IsTouchingLayers(obstacleLayers)) {
            if (useHardStopOnTurnAround) {
                rigidBody2DMovement.HardStopMovement();
            }

            TurnAround();
        }
    }

    private void TurnAround() {
        Vector2 newDirection = -rigidBody2DMovement.GetMoveDirection();

        rigidBody2DMovement.SetMoveDirection(newDirection);
        lookOrientation.SetLookDirection(newDirection);
    }
}