using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// Handles claw-based interaction with grabbable objects.
/// Detects nearby targets, grabs and anchors to one, maintains the connection,
/// and releases either manually or when the connection exceeds a distance threshold.
/// </summary>
public class ClawAttachment : MonoBehaviour, IAttachment{
    public event Action ClawClosed;
    public event Action ClawOpened;
    public event Action NothingGrabbed;
    public event Action GrabbedObject;
    public event Action ReleasedObject;

    [Header("User References")]
    [SerializeField] private Transform userTransform;

    [Header("Claw References")]
    [SerializeField] private Rigidbody2D clawRigidBody2D;
    [SerializeField] private Transform clawTransform;
    [SerializeField] private GrabbableObjectDetector detector;
    [SerializeField] private HingeJoint2D clawHingeJoint2D;

    [Header("Anchor")]
    [SerializeField] private Transform anchorPoint;

    [Header("Weight Tolerance")]
    [SerializeField, Range(0f, 9999)] private float weightTolerance = 501f;

    [Header("Ignored Collisions")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField, Range(0f, 1)] private float overlapCheckInterval = 0.1f;

    [Header("Claw Movement Spring-Damper System")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 returnPosition = new Vector2(0, -0.881f);
    [SerializeField, Range(1f, 100)] private float moveInteractPointSpeed = 50f;
    [SerializeField, Range(0.04f, 1f)] private float returnPositionThreshold = 0.04f;
    [SerializeField, Range(0.01f, 20)] private float orientClawThreshold = 0.4f;
    [SerializeField, Range(0f, 2000)] private float rotationSpeed = 300f;
    [SerializeField, Range(0f, 500)] private float stiffness = 300f;
    [SerializeField, Range(0f, 50)] private float damping = 50f;
    [SerializeField, Range(0f, 500)] private float maxForce = 300f;

    private HingeJoint2D grabbedObjectHingeJoint2D;
    private GrabbableObject grabbedObject; 

    private IUsableObject usableObject;

    private Vector2 clawInteractPoint;
    private Vector2 interactPointMoveDirection;

    private float defaultClawGravityScale;

    private bool isClawClosed = false;
    private bool isInInteractMode = false;
    private bool IsMovingInteractPoint = false;

    public void SetInteractPointPosition(Vector2 direction) {
        clawInteractPoint = direction;
    }

    public void SetIsMovingInteractPoint(bool state) {
        IsMovingInteractPoint = state;
    }

    public void SetInteractPointMoveDirection(Vector2 direction) {
        interactPointMoveDirection = direction;
    }

    private void Start() {
        defaultClawGravityScale = clawRigidBody2D.gravityScale;
    }

    private void FixedUpdate() {
        if (!isInInteractMode) {
            ReturnClaw();
        }
        else if (isInInteractMode) {
            MoveAndOrientClaw();
        }

        if (grabbedObject == null || anchorPoint == null) {
            clawRigidBody2D.constraints = RigidbodyConstraints2D.None;

            return;
        }

        bool isCarryingHeavyWeight = weightTolerance < grabbedObject.GetDistributedWeight();

        if (isCarryingHeavyWeight) {
            print("isCarryingHeavyWeight");            
        }
        else {
            print("weight tolerable");
        }    
    }

    private void MoveAndOrientClaw() {
        clawHingeJoint2D.enabled = false;

        if (IsMovingInteractPoint) {
            clawRigidBody2D.gravityScale = 0f;

            Vector2 targetPosition = (Vector2) clawTransform.position + interactPointMoveDirection * moveInteractPointSpeed * Time.fixedDeltaTime;

            MoveClawToPosition(targetPosition);
        }
        else {
            clawRigidBody2D.gravityScale = defaultClawGravityScale;

            MoveClawToPosition(clawInteractPoint);
        }

        if (grabbedObject == null) {
            OrientClawAwayFromUser();
        }
    }

    private void ReturnClaw() {
        if (clawHingeJoint2D.enabled) { return; }
        clawRigidBody2D.gravityScale = defaultClawGravityScale;

        bool isPositioned = MoveClawToReturnPosition();
        bool isOriented = OrientClawToReturnRotation();

        if (isPositioned && isOriented) {
            clawHingeJoint2D.enabled = true;
        }
    }    

    private bool MoveClawToReturnPosition() {
        Vector2 targetPosition = (Vector2)userTransform.position + returnPosition;

        Vector2 clawToPositionDisplacement = targetPosition - (Vector2)clawTransform.position;

        if (clawToPositionDisplacement.magnitude < returnPositionThreshold) {
            return true;
        }

        MoveClawToPosition(targetPosition);

        return false;
    }

    private void OrientClawAwayFromUser() {
        Vector2 clawToUserDisplacement = (Vector2)clawTransform.position - (Vector2)userTransform.position;

        if (clawToUserDisplacement.magnitude < orientClawThreshold) { return; }

        float angle = Mathf.Atan2(clawToUserDisplacement.y, clawToUserDisplacement.x) * Mathf.Rad2Deg + 90f;

        float newAngle = Mathf.MoveTowardsAngle(clawRigidBody2D.rotation, angle, rotationSpeed * Time.fixedDeltaTime);

        clawRigidBody2D.MoveRotation(newAngle);
    }

    private bool OrientClawToReturnRotation() {
        if (clawRigidBody2D.rotation > 360 || clawRigidBody2D.rotation < -360) {
            clawRigidBody2D.rotation %= 360;
        }

        float signedAngle = Mathf.DeltaAngle(0f, clawRigidBody2D.rotation);

        clawRigidBody2D.rotation = signedAngle;

        float targetAngle = 0f;

        float newAngle = Mathf.MoveTowards(signedAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);

        clawRigidBody2D.MoveRotation(newAngle);

        return Mathf.Abs(Mathf.DeltaAngle(clawRigidBody2D.rotation, targetAngle)) < 0.1f;
    }

    private void MoveClawToPosition(Vector2 position) {
        Vector2 clawToPositionDisplacement = position - (Vector2)clawTransform.position;

        Vector2 stiffnessForce = clawToPositionDisplacement * stiffness;
        Vector2 dampingForce = -1 * damping * clawRigidBody2D.linearVelocity;

        Vector2 force = stiffnessForce + dampingForce;

        force = Vector2.ClampMagnitude(force, maxForce);

        clawRigidBody2D.AddForce(force);
    }

    private void Grab() {
        ClawClosed?.Invoke();

        if (grabbedObject != null || anchorPoint == null) { return; }

        if (detector.ObjectsInRange.Count <= 0) {
            NothingGrabbed?.Invoke();

            return;
        }

        grabbedObject = detector.GetFirstGrabbableObject();

        IgnoredCollisionUtility.IgnoreCollisions(userTransform, grabbedObject.transform, true);

        grabbedObjectHingeJoint2D = grabbedObject.CreateHingeJoint2D(clawRigidBody2D, anchorPoint.position);

        GrabbedObject?.Invoke();
    }

    private void Release() {
        ClawOpened?.Invoke();

        if (grabbedObject == null) { return; }

        TryRestoreCollisions();

        ReleaseGrabbedObject();

        ReleasedObject?.Invoke();
    }

    private void ReleaseGrabbedObject() {
        grabbedObject.DestroyHingeJoint2D(grabbedObjectHingeJoint2D);

        grabbedObjectHingeJoint2D = null;
        grabbedObject = null;

        usableObject = null;
    }

    private void TryRestoreCollisions() {
        bool areObjectsOverlapping = IgnoredCollisionUtility.CheckForOverlappingObjects(userTransform, grabbedObject.transform, targetLayer);

        if (areObjectsOverlapping) {
            StartCoroutine(
                IgnoredCollisionUtility.EnableCollisionsWhenSeparated(
                    userTransform,
                    grabbedObject.transform,
                    targetLayer,
                    overlapCheckInterval
                )
            );
        }
        else {
            IgnoredCollisionUtility.IgnoreCollisions(userTransform, grabbedObject.transform, false);
        }
    }

    private void ToggleClawState() {
        isClawClosed = !isClawClosed;

        if (isClawClosed) {
            Grab();
        }
        else {
            Release();
        }
    }

    private void TryUseGrabbedObject() {
        if (grabbedObject == null) { return; }

        usableObject = grabbedObject.GetComponent<IUsableObject>();

        if (usableObject == null) { return; }

        usableObject.Use();
    }

    public void PrimaryUse() {
        ToggleClawState();
    }

    public void SecondaryUse() {
        TryUseGrabbedObject();
    }

    public void ToggleInteractMode() {
        isInInteractMode = !isInInteractMode;

        if (!isInInteractMode) {
            Release();
            isClawClosed = false;
        }
    }
}