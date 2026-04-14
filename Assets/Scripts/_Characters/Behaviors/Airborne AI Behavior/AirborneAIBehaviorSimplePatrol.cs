using System;
using UnityEngine;

/// <summary>
/// Patrols through the air by moving in a set direction and changing 
/// direction when obstacles are detected in front, above, or below.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(Rigidbody2DMovement))]
public class AirborneAIBehaviorSimplePatrol : AIBehavior {
    public override bool RequiresLookOrientation => true;
    public override bool RequiresRigidbody2DMovement => true;

    [Header("Movement")]
    [SerializeField] private Vector2 startingMoveDirection = Vector2.right;
    [SerializeField, Range(0.01f, 1f)] private float stopCheckInterval = 0.1f;

    [Header("Obstacle Detection")]
    [SerializeField] private bool useObstacleInFrontDetection = true;
    [SerializeField] private Collider2D obstacleInFrontDetection;
    [SerializeField] private bool useObstacleAboveDetection = true;
    [SerializeField] private Collider2D obstacleAboveDetection;
    [SerializeField] private bool useObstacleBelowDetection = true;
    [SerializeField] private Collider2D obstacleBelowDetection;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private bool useHardStopAtObstacle = true;
    [SerializeField, Range(0.01f, 1f)] private float detectionInterval = 0.1f;

    private LookOrientation lookOrientation;
    private Rigidbody2DMovement rigidBody2DMovement;

    private float stopCheckTimer;
    private float detectionTimer;

    private const float MIN_MOVE_THRESHOLD = 0.001f;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
    }

    private void Start() {
        rigidBody2DMovement.SetMoveDirection(startingMoveDirection);
        lookOrientation.SetLookDirection(startingMoveDirection);
    }

    public override void UpdateBehavior() {
        stopCheckTimer += Time.deltaTime;

        if (stopCheckTimer >= stopCheckInterval) {
            CheckAndResetStoppedMovement();

            stopCheckTimer = 0f;
        }

        detectionTimer += Time.deltaTime;

        if (detectionTimer >= detectionInterval) {
            ReflectMoveDirectionAtObstacle();

            detectionTimer = 0f;
        }
    }

    private void CheckAndResetStoppedMovement() {

        Vector2 currentMoveDirection = rigidBody2DMovement.MoveDirection;

        if (currentMoveDirection == Vector2.zero) {
            currentMoveDirection = startingMoveDirection;

            rigidBody2DMovement.SetMoveDirection(currentMoveDirection);
            lookOrientation.SetLookDirection(currentMoveDirection);
        }
    }

    private void ReflectMoveDirectionAtObstacle() {
        if (useObstacleInFrontDetection && obstacleInFrontDetection != null && 
            obstacleInFrontDetection.IsTouchingLayers(obstacleLayers)) {
            if (useHardStopAtObstacle) {
                rigidBody2DMovement.HardStopVelocity();
            }

            ReflectMoveDirectionHorizontally();
        }

        if (useObstacleAboveDetection && obstacleAboveDetection != null &&
            obstacleAboveDetection.IsTouchingLayers(obstacleLayers)) {
            if (useHardStopAtObstacle) {
                rigidBody2DMovement.HardStopVelocity();
            }

            MoveVerticallyDown();
        }

        if (useObstacleBelowDetection && obstacleBelowDetection != null &&
            obstacleBelowDetection.IsTouchingLayers(obstacleLayers)) {
            if (useHardStopAtObstacle) {
                rigidBody2DMovement.HardStopVelocity();
            }

            MoveVerticallyUp();
        }
    }

    private void ReflectMoveDirectionHorizontally() {
        Vector2 currentMoveDirection = rigidBody2DMovement.MoveDirection;
        Vector2 reflectedMoveDirection = new Vector2(-currentMoveDirection.x, currentMoveDirection.y);

        MoveAndOrientTowardDirection(reflectedMoveDirection);
    }

    private void MoveVerticallyDown() {
        Vector2 currentMoveDirection = rigidBody2DMovement.MoveDirection;
        Vector2 newMoveDirection = new Vector2(currentMoveDirection.x, -Math.Abs(currentMoveDirection.y));

        MoveAndOrientTowardDirection(newMoveDirection);
    }

    private void MoveVerticallyUp() {
        Vector2 currentMoveDirection = rigidBody2DMovement.MoveDirection;
        Vector2 newMoveDirection = new Vector2(currentMoveDirection.x, Math.Abs(currentMoveDirection.y));

        MoveAndOrientTowardDirection(newMoveDirection);
    }

    private void MoveAndOrientTowardDirection(Vector2 direction) {
        rigidBody2DMovement.SetMoveDirection(direction);
        lookOrientation.SetLookDirection(direction);
    }
}