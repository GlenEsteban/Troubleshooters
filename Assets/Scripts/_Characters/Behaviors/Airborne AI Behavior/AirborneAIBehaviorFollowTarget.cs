using UnityEngine;

/// <summary>
/// Controls an airborne target following AI behavior that navigates and moves
/// through the air 
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(NavMeshRigidbody2DMovement))]
public class AirborneAIBehaviorFollowTarget : AIBehavior {
    public override bool RequiresLookOrientation => true;
    public override bool RequiresNavMeshRigidbody2DMovement => true;

    [Header("Timing")]
    [SerializeField, Range(0.01f, 1f)] private float followingInterval = 0.1f;

    [Header("Target Following")]
    [SerializeField] private Transform target;
    [Range(0.2f, 10f), SerializeField] private float stoppingDistance = 1f;

    private LookOrientation lookOrientation;
    private NavMeshRigidbody2DMovement navMeshRigidbody2DMovement;

    private float horizontalDistanceToTarget;

    private float followingTargetTimer;


    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        navMeshRigidbody2DMovement = GetComponent<NavMeshRigidbody2DMovement>();
    }

    public override void UpdateBehavior() {
        if (target == null) { return; }

        followingTargetTimer += Time.deltaTime;

        if (followingTargetTimer >= followingInterval) {
            UpdateHorizontalDistanceToTarget();

            if (horizontalDistanceToTarget > stoppingDistance) {
                MoveAndOrientToTarget();
            }
            else {
                navMeshRigidbody2DMovement.StopNavMeshNavigation();
            }

            followingTargetTimer = 0f;
        }
    }

    private void UpdateHorizontalDistanceToTarget() {
        float signedHorizontalDistanceToTarget = target.position.x - transform.position.x;

        horizontalDistanceToTarget = Mathf.Abs(signedHorizontalDistanceToTarget);
    }

    private void MoveAndOrientToTarget() {
        navMeshRigidbody2DMovement.MoveToTarget(target.position);

        Vector2 selfToTargetDirection = (target.position - transform.position).normalized;

        lookOrientation.SetLookDirection(selfToTargetDirection);
    }
}