using System.Collections.Generic;
using UnityEngine;

public class GroundedAIController : MonoBehaviour {
    [Header("General")]
    [SerializeField] private GroundedBehavior _groundedBehavior;

    [Header("Simple Patrol Behavior")]
    [SerializeField] private bool hasEdgeDetection = true;
    [SerializeField] private bool hasObstacleDetection = true;
    [SerializeField] private bool hasHardStopOnObstacleDetection = true;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private Collider2D obstacleDetection;
    [SerializeField] private Collider2D groundDetection;
    [SerializeField] private Collider2D edgeDetection;
    [SerializeField] private Vector2 startingMoveDirection = Vector2.right;
    [SerializeField] private float detectionRate = 0.1f;

    [Header("Advance Patrol Behavior")]
    [SerializeField] private bool hasStopAtPatrolPoint = true;
    [SerializeField] private bool hasEdgeDetectionWhilePursuingPatrolPoint = true;
    [SerializeField] private bool hasObstacleDetectionWhilePursuingPatrolPoint = true;    
    [SerializeField] private List<Transform> patrolPoints;
    [Range(0f, 30f), SerializeField] private float waitTimeAtPatrolPoint = 3f;
    [Range(0.2f, 10f), SerializeField] private float stoppingDistance = 1f;

    [Header("Follow Target Behavior")]
    [SerializeField] private bool hasStopAtTarget = true;
    [SerializeField] private bool hasEdgeDetectionWhileFollowingTarget = true;    
    [SerializeField] private Transform target;
    [SerializeField] private float targetingRatePerSec = 0.1f;
    [Range(0.2f, 10f), SerializeField] private float followingDistance = 1f;

    private LookOrientation lookOrientation;
    private Rigidbody2DMovement rigidBody2DMovement;
    private GrabbableObject grabbableObject;

    private bool isGrounded;

    private float detectionTimer;
    private float waitAtPatrolPointTimer;
    private float followingTargetTimer;

    public Transform targetPatrolPoint;
    private int targetPatrolPointIndex;
    private Vector2 moveDirection;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
        grabbableObject = GetComponent<GrabbableObject>();
    }

    private void OnDisable() {
        if (grabbableObject == null) { return; }

        grabbableObject.Grabbed -= DisableControls;
        grabbableObject.Released -= EnableControls;
    }

    private void Start() {
        // Subscribe controller access to grabbable object events
        if (grabbableObject != null) {
            grabbableObject.Grabbed += DisableControls;
            grabbableObject.Released += EnableControls;
        }

        // Set transform as default patrol point if there are none
        if (patrolPoints == null || patrolPoints.Count == 0) {
            patrolPoints = new List<Transform> { transform };
        }

        // Set first patrol point as target patrol point
        targetPatrolPoint = patrolPoints[0];
        targetPatrolPointIndex = 0;
    }

    private void Update() {
        isGrounded = groundDetection.IsTouchingLayers(groundLayers);
        rigidBody2DMovement.SetCanMove(isGrounded); // Enable controls only when grounded

        if (!isGrounded) { return; }

        switch (_groundedBehavior) {
            case GroundedBehavior.None:
                rigidBody2DMovement.SetMoveDirection(Vector2.zero);
                break;
            case GroundedBehavior.SimplePatrol:
                SimplePatrolBehavior();
                break;
            case GroundedBehavior.AdvancePatrol:
                AdvancePatrolBehavior();
                break;
            case GroundedBehavior.FollowTarget:
                FollowTargetBehavior();
                break;
        }
    }

    private void SimplePatrolBehavior() {
        if (isGrounded) {
            detectionTimer += Time.deltaTime;
        }

        if (rigidBody2DMovement.GetMoveDirection() == Vector2.zero) {
            rigidBody2DMovement.SetMoveDirection(startingMoveDirection);
        }

        if (detectionTimer > detectionRate) {
            TurnAroundAtEdge();
            TurnAroundAtObstacle();

            detectionTimer = 0;
        }
    }

    private void TurnAroundAtEdge() {
        if (hasEdgeDetection && !edgeDetection.IsTouchingLayers(groundLayers)) {
            rigidBody2DMovement.HardStopMovement();

            Vector2 newDirection = -rigidBody2DMovement.GetMoveDirection();
            rigidBody2DMovement.SetMoveDirection(newDirection);
            lookOrientation.SetLookDirection(newDirection);
        }
    }

    private void TurnAroundAtObstacle() {
        if (hasObstacleDetection && obstacleDetection.IsTouchingLayers(obstacleLayers)) {
            if (hasHardStopOnObstacleDetection) {
                rigidBody2DMovement.HardStopMovement();
            }

            Vector2 newDirection = -rigidBody2DMovement.GetMoveDirection();
            rigidBody2DMovement.SetMoveDirection(newDirection);
            lookOrientation.SetLookDirection(newDirection);
        }
    }

    private void AdvancePatrolBehavior() {
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

            if (hasStopAtPatrolPoint) {
                rigidBody2DMovement.SetMoveDirection(Vector2.zero);
            }
        }

        if (horizontalDistanceToTargetPatrolPoint > stoppingDistance) {
            moveDirection = signedHorizontalDistanceToTargetPatrolPoint > 0 ? Vector2.right : Vector2.left;
            rigidBody2DMovement.SetMoveDirection(moveDirection);
            lookOrientation.SetLookDirection(moveDirection);
        }

        if (hasEdgeDetectionWhilePursuingPatrolPoint && !edgeDetection.IsTouchingLayers(groundLayers)) {
            if (rigidBody2DMovement.GetMoveDirection() == Vector2.zero) { return; }

            rigidBody2DMovement.HardStopMovement();
            rigidBody2DMovement.SetMoveDirection(Vector2.zero);
        }

        if (hasObstacleDetectionWhilePursuingPatrolPoint && obstacleDetection.IsTouchingLayers(obstacleLayers)) {
            if (rigidBody2DMovement.GetMoveDirection() == Vector2.zero) { return; }

            rigidBody2DMovement.HardStopMovement();
            rigidBody2DMovement.SetMoveDirection(Vector2.zero);
        }
    }

    private void FollowTargetBehavior() {
        if (isGrounded) {
            followingTargetTimer += Time.deltaTime;
        }

        if (followingTargetTimer > targetingRatePerSec) {
            float signedHorizontalDistanceToTarget = target.position.x - transform.position.x;
            float horizontalDistanceToTarget = Mathf.Abs(signedHorizontalDistanceToTarget);
            if (horizontalDistanceToTarget > followingDistance) {
                moveDirection = signedHorizontalDistanceToTarget > 0 ? Vector2.right : Vector2.left;
                rigidBody2DMovement.SetMoveDirection(moveDirection);
                lookOrientation.SetLookDirection(moveDirection);
            }

            if (hasStopAtTarget && horizontalDistanceToTarget < followingDistance) {
                rigidBody2DMovement.SetMoveDirection(Vector2.zero);
            }

            followingTargetTimer = 0;
        }

        if (hasEdgeDetectionWhileFollowingTarget && !edgeDetection.IsTouchingLayers(groundLayers)) {
            if (rigidBody2DMovement.GetMoveDirection() == Vector2.zero) { return; }
            
            rigidBody2DMovement.HardStopMovement();            
            rigidBody2DMovement.SetMoveDirection(Vector2.zero);
        }
    }

    public void EnableControls() {
        rigidBody2DMovement.SetCanMove(true);
    }

    public void DisableControls() {
        rigidBody2DMovement.HardStopMovement();
        rigidBody2DMovement.SetCanMove(false);
    }
}

public enum GroundedBehavior {
    None,
    SimplePatrol,
    AdvancePatrol,
    FollowTarget
}