using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements airborne AI Behavior that has the character patrol through 
/// airborne patrol points in sequence, moving toward each point
/// and waiting briefly before advancing to the next.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(NavMeshRigidbody2DMovement))]
public class AirborneAIBehaviorAdvancePatrol : AIBehavior {
    public override bool RequiresLookOrientation => true;
    public override bool RequiresNavMeshRigidbody2DMovement => true;

    [Header("Patrol Points")]
    [SerializeField] private List<Transform> patrolPoints;
    [Range(0f, 30f), SerializeField] private float waitTimeAtPatrolPoint = 3f;
    [Range(0.2f, 10f), SerializeField] private float stoppingDistance = 1f;
    [SerializeField, Range(0.01f, 1f)] private float distanceCheckInterval = 0.1f;
    [SerializeField, Range(0.01f, 1f)] private float patrollingInterval = 0.1f;

    private LookOrientation lookOrientation;
    private NavMeshRigidbody2DMovement navMeshRigidbody2DMovement;

    private Transform targetPatrolPoint;
    private int targetPatrolPointIndex;

    float distanceFromPatrolPoint;

    private float distanceCheckTimer;
    private float patrollingTimer;
    private float waitAtPatrolPointTimer;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        navMeshRigidbody2DMovement = GetComponent<NavMeshRigidbody2DMovement>();
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
        distanceCheckTimer += Time.deltaTime;

        if (distanceCheckTimer > distanceCheckInterval) {
            distanceFromPatrolPoint = Vector2.Distance((Vector2)targetPatrolPoint.position, (Vector2)transform.position);

            distanceCheckTimer = 0f;
        }

        if (distanceFromPatrolPoint >= stoppingDistance) {
            patrollingTimer += Time.deltaTime;

            if (patrollingTimer > patrollingInterval) {
                MoveAndOrientToTargetPatrolPoint();

                patrollingTimer = 0f;
            }
        }
        else {
            waitAtPatrolPointTimer += Time.deltaTime;

            if (waitAtPatrolPointTimer > waitTimeAtPatrolPoint) {
                AdvanceToNextPatrolPoint();

                waitAtPatrolPointTimer = 0f;
            }
        }
    }

    private void MoveAndOrientToTargetPatrolPoint() {
        navMeshRigidbody2DMovement.MoveToTarget(targetPatrolPoint.position);

        Vector2 currentMoveDirection = navMeshRigidbody2DMovement.MoveDirection;

        lookOrientation.SetLookDirection(currentMoveDirection);
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
}