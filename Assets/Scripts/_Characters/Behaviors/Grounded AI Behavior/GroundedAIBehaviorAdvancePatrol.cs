using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a grounded advance patrol AI behavior that moves along the ground 
/// between patrol points in sequence and optionally uses edge and obstacle detection
/// to restrict movement.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(Rigidbody2DMovement))]
public class GroundedAIBehaviorAdvancePatrol : AIBehavior {
    public override bool RequiresLookOrientation => true;
    public override bool RequiresRigidbody2DMovement => true;

    [Header("Timing")]
    [SerializeField, Range(0.01f, 1f)] private float distanceCheckInterval = 0.1f;
    [SerializeField, Range(0.01f, 1f)] private float patrollingInterval = 0.1f;
    [SerializeField, Range(0.01f, 1f)] private float detectionInterval = 0.1f;

    [Header("Patrol Points")]
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] private bool useHardStopAtPatrolPoint = false;
    [Range(0.2f, 10f), SerializeField] private float stoppingDistance = 1f;
    [Range(0f, 30f), SerializeField] private float waitTimeAtPatrolPoint = 3f;

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

    private Transform targetPatrolPoint;
    private int targetPatrolPointIndex;

    private float signedHorizontalDistanceToTargetPatrolPoint;
    private float horizontalDistanceToTargetPatrolPoint;

    private float distanceCheckTimer;
    private float patrollingTimer;
    private float waitAtPatrolPointTimer;
    private float detectionTimer;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
    }

    private void Start() {
        // Set transform as default patrol point if there are none
        if (patrolPoints == null || patrolPoints.Count == 0) {
            patrolPoints = new List<Transform> { transform };
        }

        // Set first patrol point as target patrol point
        targetPatrolPoint = patrolPoints[0];
        targetPatrolPointIndex = 0;
    }

    public override void UpdateBehavior() {
        if (!CheckIfGrounded()) { return; }

        distanceCheckTimer += Time.deltaTime;

        if (distanceCheckTimer >= distanceCheckInterval) {
            UpdateHorizontalDistanceToTargetPatrolPoint();

            distanceCheckTimer = 0f;
        }

        if (horizontalDistanceToTargetPatrolPoint >= stoppingDistance) {
            patrollingTimer += Time.deltaTime;

            if (patrollingTimer >= patrollingInterval) {
                MoveAndOrientToTargetPatrolPoint();

                patrollingTimer = 0f;
            }
        }
        else {
            StopAtPatrolPoint();

            waitAtPatrolPointTimer += Time.deltaTime;

            if (waitAtPatrolPointTimer >= waitTimeAtPatrolPoint) {
                AdvanceToNextPatrolPoint();

                waitAtPatrolPointTimer = 0f;
            }
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

    private void AdvanceToNextPatrolPoint() {
        if (targetPatrolPointIndex < patrolPoints.Count - 1) {
            targetPatrolPointIndex++;
            targetPatrolPoint = patrolPoints[targetPatrolPointIndex];
        }
        else {
            targetPatrolPointIndex = 0;
            targetPatrolPoint = patrolPoints[0];
        }
    }

    private void UpdateHorizontalDistanceToTargetPatrolPoint() {
        signedHorizontalDistanceToTargetPatrolPoint = targetPatrolPoint.position.x - transform.position.x;
        horizontalDistanceToTargetPatrolPoint = Mathf.Abs(signedHorizontalDistanceToTargetPatrolPoint);
    }

    private void StopAtPatrolPoint() {
        if (rigidBody2DMovement.MoveDirection == Vector2.zero) { return; }

        if (useHardStopAtPatrolPoint) {
            rigidBody2DMovement.HardStopVelocity();
        }

        rigidBody2DMovement.SetMoveDirection(Vector2.zero);
    }

    private void MoveAndOrientToTargetPatrolPoint() {
        Vector2 moveDirection = signedHorizontalDistanceToTargetPatrolPoint >= 0 ? Vector2.right : Vector2.left;

        rigidBody2DMovement.SetMoveDirection(moveDirection);
        lookOrientation.SetLookDirection(moveDirection);
    }

    private void StopAtEdge() {
        if (!useEdgeDetection || edgeDetection == null) { return; }
        if (rigidBody2DMovement.MoveDirection == Vector2.zero) { return; }

        if (!edgeDetection.IsTouchingLayers(groundLayers)) {
            if (useHardStopAtEdge) {
                rigidBody2DMovement.HardStopVelocity();
            }

            rigidBody2DMovement.SetMoveDirection(Vector2.zero);
        }
    }

    private void StopAtObstacle() {
        if (!useObstacleDetection || obstacleDetection == null) { return; }
        if (rigidBody2DMovement.MoveDirection == Vector2.zero) { return; }

        if (obstacleDetection.IsTouchingLayers(obstacleLayers)) {
            if (useHardStopAtObstacle) {
                rigidBody2DMovement.HardStopVelocity();
            }

            StopMovement();
        }
    }

    private void StopMovement() {
        rigidBody2DMovement.SetMoveDirection(Vector2.zero);
    }
}