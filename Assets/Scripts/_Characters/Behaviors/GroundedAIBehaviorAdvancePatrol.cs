using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements grounded patrol behavior that moves the character along the ground
/// and pursues patrol points in specified order with optional edge/obstacle detection.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(Rigidbody2DMovement))]
public class GroundedAIBehaviorAdvancePatrol : GroundedAIBehavior {
    [Header("Patrol Points")]
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private bool useHardStopAtPatrolPoint = true;
    [Range(0f, 30f), SerializeField] private float waitTimeAtPatrolPoint = 3f;
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

    private Transform targetPatrolPoint;
    private int targetPatrolPointIndex;

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
        UpdateTargetPatrolPoint();

        UpdateMoveDirection();

        if (CheckIfGrounded()) {
            detectionTimer += Time.deltaTime;

            if (detectionTimer > detectionInterval) {
                HardStopAtEdge();
                HardStopAtObstacle();

                detectionTimer = 0;
            }
        }
    }

    private void UpdateTargetPatrolPoint() {
        float signedHorizontalDistanceToTargetPatrolPoint = targetPatrolPoint.position.x - transform.position.x;
        float horizontalDistanceToTargetPatrolPoint = Mathf.Abs(signedHorizontalDistanceToTargetPatrolPoint);

        if (horizontalDistanceToTargetPatrolPoint <= stoppingDistance) {
            waitAtPatrolPointTimer += Time.deltaTime;

            if (waitAtPatrolPointTimer > waitTimeAtPatrolPoint) {
                if (targetPatrolPointIndex < patrolPoints.Count - 1) {
                    targetPatrolPointIndex++;
                    targetPatrolPoint = patrolPoints[targetPatrolPointIndex];
                }
                else {
                    targetPatrolPointIndex = 0;
                    targetPatrolPoint = patrolPoints[0];
                }

                waitAtPatrolPointTimer = 0;
            }

            if (useHardStopAtPatrolPoint) {
                rigidBody2DMovement.SetMoveDirection(Vector2.zero);
            }
        }
    }
    private void UpdateMoveDirection() {
        float signedHorizontalDistanceToTargetPatrolPoint = targetPatrolPoint.position.x - transform.position.x;
        float horizontalDistanceToTargetPatrolPoint = Mathf.Abs(signedHorizontalDistanceToTargetPatrolPoint);

        if (horizontalDistanceToTargetPatrolPoint > stoppingDistance) {
            Vector2 moveDirection = signedHorizontalDistanceToTargetPatrolPoint > 0 ? Vector2.right : Vector2.left;

            rigidBody2DMovement.SetMoveDirection(moveDirection);
            lookOrientation.SetLookDirection(moveDirection);
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