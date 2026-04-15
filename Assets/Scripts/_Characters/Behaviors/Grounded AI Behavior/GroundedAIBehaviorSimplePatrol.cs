using UnityEngine;

/// <summary>
/// Controls a grounded simple patrol AI behavior that moves along the ground 
/// and turns around when reaching an edge or hitting an obstacle.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(Rigidbody2DMovement))]
public class GroundedAIBehaviorSimplePatrol : AIBehavior {
    public override bool RequiresLookOrientation => true;
    public override bool RequiresRigidbody2DMovement => true;

    [Header("Timing")]
    [SerializeField, Range(0.01f, 1f)] private float detectionInterval = 0.1f;

    [Header("Movement")]
    [SerializeField] private Vector2 startingMoveDirection = Vector2.right;
    [SerializeField] private bool useHardStopOnTurnAround = false;

    [Header("Ground Detection")]
    [SerializeField] private Collider2D groundDetection;
    [SerializeField] private LayerMask groundLayers;

    [Header("Edge Detection")]
    [SerializeField] private bool useEdgeDetection = true;
    [SerializeField] private Collider2D edgeDetection;

    [Header("Obstacle Detection")]
    [SerializeField] private bool useObstacleDetection = true;
    [SerializeField] private Collider2D obstacleDetection;
    [SerializeField] private LayerMask obstacleLayers;

    private LookOrientation lookOrientation;
    private Rigidbody2DMovement rigidBody2DMovement;

    private bool wasGrounded;

    private float detectionTimer;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
    }

    private void Start() {
        startingMoveDirection = startingMoveDirection.normalized;

        rigidBody2DMovement.SetMoveDirection(startingMoveDirection);
        lookOrientation.SetLookDirection(startingMoveDirection);

        wasGrounded = CheckIfGrounded();
    }

    public override void UpdateBehavior() {
        if (!CheckIfGrounded()) {
            rigidBody2DMovement.StopMovement();

            return;
        }
        else {
            ResumeMovement();
        }

        wasGrounded = true;

        detectionTimer += Time.deltaTime;

        if (detectionTimer >= detectionInterval) {
            TurnAroundAtEdge();
            TurnAroundAtObstacle();

            detectionTimer = 0f;
        }
    }

    private bool CheckIfGrounded() {
        if (groundDetection == null) { return false; }

        return groundDetection.IsTouchingLayers(groundLayers);
    }

    private void ResumeMovement() {
        if (rigidBody2DMovement.MoveDirection == Vector2.zero) {
            Vector2 facingDirection = lookOrientation.FacingDirection;

            rigidBody2DMovement.SetMoveDirection(facingDirection);
        }
    }

    private void TurnAroundAtEdge() {
        if (!useEdgeDetection || edgeDetection == null) { return; }

        if (!edgeDetection.IsTouchingLayers(groundLayers)) {
            if (useHardStopOnTurnAround) {
                rigidBody2DMovement.HardStopVelocity();
            }

            TurnAround();
        }
    }

    private void TurnAroundAtObstacle() {
        if (!useObstacleDetection || obstacleDetection == null) { return; }

        if (obstacleDetection.IsTouchingLayers(obstacleLayers)) {
            if (useHardStopOnTurnAround) {
                rigidBody2DMovement.HardStopVelocity();
            }

            TurnAround();
        }
    }

    private void TurnAround() {
        Vector2 currentMoveDirection = rigidBody2DMovement.MoveDirection;
        Vector2 newDirection = new Vector2(-currentMoveDirection.x, 0f);

        rigidBody2DMovement.SetMoveDirection(newDirection);
        lookOrientation.SetLookDirection(newDirection);
    }
}