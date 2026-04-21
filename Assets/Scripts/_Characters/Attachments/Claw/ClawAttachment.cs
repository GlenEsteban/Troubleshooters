using System;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Anchor")]
    [SerializeField] private int anchorId;
    [SerializeField] private Transform anchorPoint;

    [Header("Release")]
    [SerializeField] private float unintendedReleaseThreshold = 0.8f;

    private List<GrabbableObject> grabbableObjectsInRange = new List<GrabbableObject>();
    private GrabbableObject targetGrabbableObject;

    private IUsableObject usableObject;

    private bool isClawClosed = false;

    private void OnEnable() {
        CharacterManager.Instance.RegistryOrderUpdated += UpdateAnchorId;
    }

    private void OnDisable() {
        CharacterManager.Instance.RegistryOrderUpdated -= UpdateAnchorId;
    }

    private void Start() {
        UpdateAnchorId();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        GrabbableObject grabbableObject = collision.GetComponentInParent<GrabbableObject>();

        if (grabbableObject == null) { return; }

        if (grabbableObject == GetComponentInParent<GrabbableObject>()) { return; }

        if (!grabbableObjectsInRange.Contains(grabbableObject)) {
            grabbableObjectsInRange.Add(grabbableObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        GrabbableObject grabbableObject = collision.GetComponentInParent<GrabbableObject>();

        if (grabbableObject == null) { return; }
        
        if (grabbableObjectsInRange.Contains(grabbableObject)) {
            grabbableObjectsInRange.Remove(grabbableObject);
        }
    }

    private void Update() {
        if (targetGrabbableObject == null || anchorPoint == null) { return; }

        targetGrabbableObject.UpdateAnchorTargetWorldPosition(anchorId, anchorPoint.position);

        CheckForUnintendedRelease();
    }
    private void UpdateAnchorId() {
        anchorId = CharacterManager.Instance.GetPlayerIndex(GetComponentInParent<Character>());
    }

    private void CheckForUnintendedRelease() {
        if ( anchorPoint == null) { return; }

        Vector2 targetAnchorWorldPos = targetGrabbableObject.GetAnchorWorldPosition(anchorId);

        float targetToAnchorPointDistance = Vector2.Distance(targetAnchorWorldPos, anchorPoint.position);

        if (targetToAnchorPointDistance > unintendedReleaseThreshold) {
            UnintendedRelease();
        }
    }

    private void Grab() {
        ClawClosed?.Invoke();

        if (targetGrabbableObject != null || anchorPoint == null ) { return; }

        if (grabbableObjectsInRange.Count <= 0) {
            NothingGrabbed?.Invoke();

            return;
        }

        // Removes any objects that were destroyed while in grab collider
        grabbableObjectsInRange.RemoveAll(item => item == null);

        if (grabbableObjectsInRange.Count <= 0) { return; }

        targetGrabbableObject = grabbableObjectsInRange[0];

        targetGrabbableObject.AddAnchorPoint(anchorId, anchorPoint.position);

        GrabbedObject?.Invoke();
    }

    private void Release() {
        ClawOpened?.Invoke();

        if (targetGrabbableObject == null) { return; }

        targetGrabbableObject.RemoveAnchorPoint(anchorId);
        targetGrabbableObject = null;

        usableObject = null;

        ReleasedObject?.Invoke();
    }

    private void UnintendedRelease() {
        if (targetGrabbableObject == null) { return; }

        targetGrabbableObject.RemoveAnchorPoint(anchorId);
        targetGrabbableObject = null;

        usableObject = null;

        UnintentionallyReleased?.Invoke();
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

        usableObject = targetGrabbableObject.GetComponent<IUsableObject>();

        if (usableObject == null) { return; }

        usableObject.Use();
    }

    public void PrimaryUse() {
        ToggleClawState();
    }

    public void SecondaryUse() {
        TryUseGrabbedObject();
    }
}