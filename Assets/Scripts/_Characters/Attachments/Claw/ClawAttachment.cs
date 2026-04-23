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
    [SerializeField] private Collider2D[] targetColliders;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField, Range(0f, 1)] private float overlapCheckInterval = 0.1f;    

    [Header("Follow Target Spring-Damper System")]
    [SerializeField, Range(0f, 50)] private float followTargetRange = 3f;
    [SerializeField, Range(0f, 50)] private float stiffness = 50f;
    [SerializeField, Range(0f, 50)] private float damping = 2f;
    [SerializeField, Range(0f, 50)] private float maxForce = 10f;

    [Header("Anchor")]
    [SerializeField] private int anchorId;
    [SerializeField] private Transform anchorPoint;

    [Header("Release")]
    [SerializeField] private float unintendedReleaseThreshold = 1f;

    private GrabbableObject targetGrabbableObject;

    private Vector3 targetPosition;

    private IUsableObject targetUsableObject;

    private bool isClawClosed = false;

    public void SetTargetPosition(Vector2 direction) {
        targetPosition = direction;
    }

    private void OnEnable() {
        CharacterManager.Instance.RegistryOrderUpdated += UpdateAnchorId;
    }

    private void OnDisable() {
        CharacterManager.Instance.RegistryOrderUpdated -= UpdateAnchorId;
    }

    private void Start() {
        UpdateAnchorId();
    }

    private void FixedUpdate() {
        MoveClawToTarget();

        if (targetGrabbableObject == null || anchorPoint == null) { return; }

        targetGrabbableObject.UpdateAnchorTargetWorldPosition(anchorId, anchorPoint.position);

        CheckForUnintendedRelease();
    }

    private void UpdateAnchorId() {
        anchorId = CharacterManager.Instance.GetPlayerIndex(GetComponentInParent<Character>());
    }

    private void MoveClawToTarget() {
        Vector2 userToTargetDisplacement = targetPosition - userTransform.position;

        if (userToTargetDisplacement.magnitude > followTargetRange) { return; }

        Vector2 clawToTargetDisplacement = targetPosition - clawTransform.position;

        Vector2 stiffnessForce = clawToTargetDisplacement * stiffness;
        Vector2 dampingForce = - clawRigidBody2D.linearVelocity * damping;

        Vector2 force = stiffnessForce + dampingForce;

        force = Vector2.ClampMagnitude(force, maxForce);

        clawRigidBody2D.AddForce(force);
    }

    private void CheckForUnintendedRelease() {
        if (anchorPoint == null) { return; }

        Vector2 targetAnchorWorldPos = targetGrabbableObject.GetAnchorWorldPosition(anchorId);

        float targetToAnchorPointDistance = Vector2.Distance(targetAnchorWorldPos, anchorPoint.position);

        if (targetToAnchorPointDistance > unintendedReleaseThreshold) {
            UnintendedRelease();
        }
    }

    private void Grab() {
        ClawClosed?.Invoke();

        if (targetGrabbableObject != null || anchorPoint == null) { return; }

        if (detector.ObjectsInRange.Count <= 0) {
            NothingGrabbed?.Invoke();

            return;
        }

        AssignFirstAvailableGrabbableObject();

        targetColliders = targetGrabbableObject.GetComponentsInChildren<Collider2D>();

        SetIgnoredCollision(userColliders, targetColliders, true);

        targetGrabbableObject.AddAnchorPoint(anchorId, anchorPoint.position);

        GrabbedObject?.Invoke();
    }

    private void AssignFirstAvailableGrabbableObject() {
        detector.RemoveDestroyedObjects();

        if (detector.ObjectsInRange.Count <= 0) { return; }

        targetGrabbableObject = detector.ObjectsInRange[0];
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

        if (targetGrabbableObject == null) { return; }

        ReleaseTarget();

        bool areObjectsOverlapping = CheckForOverlappingColliders(userColliders, targetColliders);

        if (areObjectsOverlapping) {
            StartCoroutine(CheckForOverlappingCollider(targetColliders));
        }
        else {
            SetIgnoredCollision(userColliders, targetColliders, false);
        }

        ReleasedObject?.Invoke();
    }

    private void UnintendedRelease() {
        if (targetGrabbableObject == null) { return; }

        ReleaseTarget();

        UnintentionallyReleased?.Invoke();
    }

    private void ReleaseTarget() {
        targetGrabbableObject.RemoveAnchorPoint(anchorId);
        targetGrabbableObject = null;

        targetUsableObject = null;
    }

    private IEnumerator CheckForOverlappingCollider(Collider2D[] colliders) {
        bool areObjectsOverlapping = true;

        while (areObjectsOverlapping) {
            areObjectsOverlapping = CheckForOverlappingColliders(userColliders, colliders);

            yield return new WaitForSeconds(overlapCheckInterval);
        }

        SetIgnoredCollision(userColliders, colliders, false);
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
        if (targetGrabbableObject == null) { return; }

        targetUsableObject = targetGrabbableObject.GetComponent<IUsableObject>();

        if (targetUsableObject == null) { return; }

        targetUsableObject.Use();
    }

    public void PrimaryUse() {
        ToggleClawState();
    }

    public void SecondaryUse() {
        TryUseGrabbedObject();
    }
}