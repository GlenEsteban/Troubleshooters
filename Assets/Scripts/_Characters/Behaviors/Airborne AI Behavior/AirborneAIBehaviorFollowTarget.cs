using UnityEngine;

/// <summary>
/// Implements grounded follow behavior that moves the character along the ground
/// toward a target with optional edge/obstacle detection.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(NavMeshRigidbody2DMovement))]
public class AirborneAIBehaviorFollowTarget : AIBehavior {
    public override bool RequiresLookOrientation => true;
    public override bool RequiresRigidbody2DMovement => true;

    [Header("Target Following")]
    [SerializeField] private Transform target;

    private LookOrientation lookOrientation;
    private NavMeshRigidbody2DMovement navMeshRigidbody2DMovement;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        navMeshRigidbody2DMovement = GetComponent<NavMeshRigidbody2DMovement>();
    }

    public override void UpdateBehavior() {
        navMeshRigidbody2DMovement.MoveToTarget(target.position);

        Vector2 selfToTargetDirection = (target.position - transform.position).normalized;
        lookOrientation.SetLookDirection(selfToTargetDirection);
    }
}