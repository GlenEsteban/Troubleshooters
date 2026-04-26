using System;
using System.Collections;
using TMPro;
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

    [Header("User")]
    [SerializeField] private Transform userTransform;
    [SerializeField] private Collider2D[] userColliders;

    [Header("Claw")]
    [SerializeField] private Rigidbody2D clawRigidBody2D;
    [SerializeField] private Transform clawTransform;
    [SerializeField] private GrabbableObjectDetector detector;

    [Header("Ignored Collisions")]
    [SerializeField] private Collider2D[] grabbedObjectColliders;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField, Range(0f, 1)] private float overlapCheckInterval = 0.1f;    

    [Header("Interact Mode Spring-Damper System")]
    [SerializeField, Range(0f, 500)] private float stiffness = 50f;
    [SerializeField, Range(0f, 50)] private float damping = 2f;
    [SerializeField, Range(0f, 500)] private float maxForce = 10f;

    [Header("Anchor")]
    [SerializeField] private int anchorId;
    [SerializeField] private Transform anchorPoint;

    [Header("Release")]
    [SerializeField] private float unintendedReleaseThreshold = 1f;

    private GrabbableObject grabbedObject;
    private RelativeJoint2D relativeJoint2D;

    private Vector2 clawLookDirection;

    private IUsableObject usableObject;

    private bool isClawClosed = false;
    private bool isInInteractMode = false;

    public void SetTargetPosition(Vector2 direction) {
        clawLookDirection = direction;
    }

    private void OnEnable() {
        CharacterManager.Instance.RegistryOrderUpdated += UpdateAnchorId;
    }

    private void OnDisable() {
        CharacterManager.Instance.RegistryOrderUpdated -= UpdateAnchorId;
    }

    private void Awake() {
        relativeJoint2D = GetComponentInChildren<RelativeJoint2D>();
    }

    private void Start() {
        UpdateAnchorId();
    }

    private void FixedUpdate() {
        // Enable relative joint when not following target
        relativeJoint2D.enabled = !isInInteractMode;

        if (isInInteractMode) {
            MoveClawToTarget();
            OrientClawToTarget();
        }

        if (grabbedObject == null || anchorPoint == null) { return; }

        grabbedObject.UpdateAnchorTargetWorldPosition(anchorId, anchorPoint.position);

        CheckForUnintendedRelease();
    }

    private void UpdateAnchorId() {
        anchorId = CharacterManager.Instance.GetPlayerIndex(GetComponentInParent<Character>());
    }

    private void MoveClawToTarget() {
        Vector2 clawToTargetDisplacement = clawLookDirection - (Vector2) clawTransform.position;

        Vector2 stiffnessForce = clawToTargetDisplacement * stiffness;
        Vector2 dampingForce = -clawRigidBody2D.linearVelocity * damping;

        Vector2 force = stiffnessForce + dampingForce;

        force = Vector2.ClampMagnitude(force, maxForce);

        clawRigidBody2D.AddForce(force);
    }

    private void OrientClawToTarget() {
        Vector2 clawToUserDisplacement = clawLookDirection - (Vector2) userTransform.position;

        float angle = Mathf.Atan2(clawToUserDisplacement.y, clawToUserDisplacement.x) * Mathf.Rad2Deg + 90f;

        clawRigidBody2D.MoveRotation(angle);
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

        SetIgnoredCollision(userColliders, grabbedObjectColliders, true);

        grabbedObject.AddAnchorPoint(anchorId, anchorPoint.position);

        GrabbedObject?.Invoke();
    }

    private void AssignGrabbedObject() {
        detector.RemoveDestroyedObjects();

        if (detector.ObjectsInRange.Count <= 0) { return; }

        grabbedObject = detector.ObjectsInRange[0];

        grabbedObjectColliders = grabbedObject.GetComponentsInChildren<Collider2D>();
    }

    private void SetIgnoredCollision(Collider2D[] collidersA, Collider2D[] collidersB, bool ignore) {
        foreach (Collider2D colliderA in collidersA) {
            if (colliderA == null || colliderA.isTrigger) { continue; }

            foreach (Collider2D colliderB in collidersB) {
                if (colliderB == null || colliderB.isTrigger) { continue; }

                Physics2D.IgnoreCollision(colliderA, colliderB, ignore);
            }
        }
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

    private void RunCheckForOverlappingColliders() {
        bool areObjectsOverlapping = CheckForOverlappingColliders(userColliders, grabbedObjectColliders);

        if (areObjectsOverlapping) {
            StartCoroutine(CheckForOverlappingCollider(grabbedObjectColliders));
        }
        else {
            SetIgnoredCollision(userColliders, grabbedObjectColliders, false);
        }
    }

    private IEnumerator CheckForOverlappingCollider(Collider2D[] colliders) {
        bool areObjectsOverlapping = true;

        while (areObjectsOverlapping) {
            areObjectsOverlapping = CheckForOverlappingColliders(userColliders, colliders);

            Debug.Log("checking for overlaps");

            yield return new WaitForSeconds(overlapCheckInterval);
        }

        SetIgnoredCollision(userColliders, colliders, false);

        Debug.Log("ended checking");
    }

    private void ReleaseGrabbedObject() {
        grabbedObject.RemoveAnchorPoint(anchorId);
        grabbedObject = null;

        usableObject = null;
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
        if(grabbedObject == null) {
            isInInteractMode = !isInInteractMode;
        }

        TryUseGrabbedObject();
    }
}