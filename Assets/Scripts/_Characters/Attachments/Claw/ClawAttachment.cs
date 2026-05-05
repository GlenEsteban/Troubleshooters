using System;
using System.Collections;
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
    public event Action UnintentionallyReleased;

    [Header("User References")]
    [SerializeField] private Transform userTransform;
    [SerializeField] private Collider2D[] userColliders;

    [Header("Claw References")]
    [SerializeField] private Rigidbody2D clawRigidBody2D;
    [SerializeField] private Transform clawTransform;
    [SerializeField] private GrabbableObjectDetector detector;

    [Header("Anchor")]
    [SerializeField] private int anchorId;
    [SerializeField] private Transform anchorPoint;
    [SerializeField] private float unintendedReleaseThreshold = 1f;

    [Header("Weight Tolerance")]
    [SerializeField, Range(0f, 9999)] private float weightTolerance = 501f;

    [Header("Ignored Collisions")]
    [SerializeField] private Collider2D[] grabbedObjectColliders;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField, Range(0f, 1)] private float overlapCheckInterval = 0.1f;

    [Header("Claw Movement Spring-Damper System")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 returnPosition = new Vector2(0, -0.931f);
    [SerializeField, Range(1f, 100)] private float moveInteractPointSpeed = 50f;
    [SerializeField, Range(0.04f, 20)] private float returnPositionThreshold = 0.04f;
    [SerializeField, Range(0.01f, 20)] private float orientClawThreshold = 0.4f;
    [SerializeField, Range(0f, 2000)] private float rotationSpeed = 300f;
    [SerializeField, Range(0f, 500)] private float stiffness = 300f;
    [SerializeField, Range(0f, 50)] private float damping = 50f;
    [SerializeField, Range(0f, 500)] private float maxForce = 300f;

    private GrabbableObject grabbedObject;
    private HingeJoint2D hingeJoint2D;
    private ObjectCollisionPhasing objectCollisionPhasing;

    private Vector2 clawInteractPoint;
    private Vector2 interactPointMoveDirection;

    private IUsableObject usableObject;

    private float defaultClawGravityScale;

    private bool isClawClosed = false;
    private bool isInInteractMode = false;
    private bool isUsingStickBasedClawMovement = false;

    public void SetIsMovingInteractPoint(bool state) {
        isUsingStickBasedClawMovement = state;
    }

    public void SetClawInteractPoint(Vector2 direction) {
        clawInteractPoint = direction;
    }

    public void SetInteractPointMoveDirection(Vector2 direction) {
        interactPointMoveDirection = direction;
    }

    private void OnEnable() {
        CharacterManager.Instance.RegistryOrderUpdated += UpdateAnchorId;
    }

    private void OnDisable() {
        CharacterManager.Instance.RegistryOrderUpdated -= UpdateAnchorId;
    }

    private void Awake() {
        hingeJoint2D = GetComponentInChildren<HingeJoint2D>();

        objectCollisionPhasing = GetComponent<ObjectCollisionPhasing>();
    }
    private void Start() {
        UpdateAnchorId();

        defaultClawGravityScale = clawRigidBody2D.gravityScale;
    }

    private void UpdateAnchorId() {
        anchorId = CharacterManager.Instance.GetPlayerIndex(GetComponentInParent<Character>());
    }

    private void FixedUpdate() {
        if (isInInteractMode) {
            hingeJoint2D.enabled = false;

            if (isUsingStickBasedClawMovement) {
                clawRigidBody2D.gravityScale = 0f;

                Vector2 targetPosition = (Vector2)clawTransform.position + interactPointMoveDirection * moveInteractPointSpeed * Time.fixedDeltaTime;

                MoveClawToPosition(targetPosition);
            }
            else {
                clawRigidBody2D.gravityScale = defaultClawGravityScale;

                MoveClawToPosition(clawInteractPoint);
            }

            OrientClawAwayFromUser();
        }
        else if (!hingeJoint2D.enabled) {
            clawRigidBody2D.gravityScale = defaultClawGravityScale;

            bool isPositioned = MoveClawToReturnPosition();
            bool isOriented = OrientClawToReturnRotation();

            if (isPositioned && isOriented) {
                hingeJoint2D.enabled = true;
            }
        }

        if (grabbedObject == null || anchorPoint == null) {
            clawRigidBody2D.constraints = RigidbodyConstraints2D.None;

            return;
        }

        bool isCarryingHeavyWeight = weightTolerance < grabbedObject.GetDistributedWeight();

        if (isCarryingHeavyWeight) {
            hingeJoint2D.enabled = false;

            clawRigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;

            print("isCarryingHeavyWeight");
            return;
        }
        else {
            clawRigidBody2D.constraints = RigidbodyConstraints2D.None;
            print("weight tolerable");
        }

        grabbedObject.UpdateAnchorTargetWorldPosition(anchorId, anchorPoint.position);

        CheckForUnintendedRelease();
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


    private void CheckForUnintendedRelease() {
        if (anchorPoint == null) { return; }

        Vector2 targetAnchorWorldPos = grabbedObject.GetAnchorWorldPosition(anchorId);

        float targetToAnchorPointDistance = Vector2.Distance(targetAnchorWorldPos, anchorPoint.position);

        if (targetToAnchorPointDistance > unintendedReleaseThreshold) {
            UnintendedRelease();
        }
    }

    private void Grab() {
        ClawClosed?.Invoke();

        if (grabbedObject != null || anchorPoint == null) { return; }

        if (detector.ObjectsInRange.Count <= 0) {
            NothingGrabbed?.Invoke();

            return;
        }

        AssignGrabbedObject();

        objectCollisionPhasing.SetIgnoredCollision(userColliders, grabbedObjectColliders, true);

        grabbedObject.AddAnchorPoint(anchorId, anchorPoint.position);

        GrabbedObject?.Invoke();
    }

    private void AssignGrabbedObject() {
        grabbedObject = detector.GetFirstGrabbableObject();

        if (grabbedObject == null) { return; }

        grabbedObjectColliders = grabbedObject.GetComponentsInChildren<Collider2D>();
    }



    private void Release() {
        ClawOpened?.Invoke();

        if (grabbedObject == null) { return; }

        ReleaseGrabbedObject();

        RunCheckForOverlappingColliders();

        ReleasedObject?.Invoke();
    }

    private void UnintendedRelease() {
        if (grabbedObject == null) { return; }

        ReleaseGrabbedObject();

        RunCheckForOverlappingColliders();

        UnintentionallyReleased?.Invoke();
    }

    private void ReleaseGrabbedObject() {
        grabbedObject.RemoveAnchorPoint(anchorId);
        grabbedObject = null;

        usableObject = null;
    }

    private void RunCheckForOverlappingColliders() {
        bool areObjectsOverlapping = CheckForOverlappingColliders(userColliders, grabbedObjectColliders);

        if (areObjectsOverlapping) {
            StartCoroutine(CheckForOverlappingCollider(grabbedObjectColliders));
        }
        else {
            objectCollisionPhasing.SetIgnoredCollision(userColliders, grabbedObjectColliders, false);
        }
    }

    private IEnumerator CheckForOverlappingCollider(Collider2D[] colliders) {
        bool areObjectsOverlapping = true;

        while (areObjectsOverlapping) {
            areObjectsOverlapping = CheckForOverlappingColliders(userColliders, colliders);

            yield return new WaitForSeconds(overlapCheckInterval);
        }

        objectCollisionPhasing.SetIgnoredCollision(userColliders, colliders, false);
    }

    private bool CheckForOverlappingColliders(Collider2D[] collidersA, Collider2D[] collidersB) {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useTriggers = false;

        Collider2D[] results = new Collider2D[10];

        foreach (Collider2D colliderA in collidersA) {
            if (colliderA == null) continue;

            int overlapCount = colliderA.Overlap(filter, results);

            for (int i = 0; i < overlapCount; i++) {
                for (int j = 0; j < collidersB.Length; j++) {
                    if (collidersB[j].isTrigger) continue;

                    if (results[i] == collidersB[j]) {
                        return true;
                    }
                }
            }
        }

        return false;
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
    }

    public void SetInteractMode(bool enabled) {
        isInInteractMode = enabled;

        if (!enabled) {
            Release();

            isUsingStickBasedClawMovement = false;
        }
    }
}