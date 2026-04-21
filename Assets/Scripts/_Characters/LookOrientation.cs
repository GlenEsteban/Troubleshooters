using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks the current look direction and controls left/right orientation by
/// horizontally flipping assigned child transforms based on that direction.
/// 
/// Do not assign transforms that are driven by physics joints, as flipping them can
/// interfere with joint behavior.
/// </summary>
public class LookOrientation : MonoBehaviour {
    public event Action<Vector2> LookDirectionChanged;
    public Vector2 LookDirection => lookDirection;
    public Vector2 FacingDirection => horizontalFacingDirection;

    [SerializeField] private List<Transform> objectsToOrient;

    private Vector2 lookDirection = Vector2.right;
    private Vector2 horizontalFacingDirection = Vector2.right;

    public void SetLookDirection(Vector2 direction) {
        if (direction == Vector2.zero) return;

        bool hasHorizontalIntent = !Mathf.Approximately(direction.x, 0f); // guards against 0
        bool isFacingDifferentDirection = direction.x * horizontalFacingDirection.x < 0f;
        bool isHorizontalFacingChanged = hasHorizontalIntent && isFacingDifferentDirection;

        lookDirection = direction.normalized;

        if (hasHorizontalIntent) {
            horizontalFacingDirection = new Vector2(Mathf.Sign(lookDirection.x), 0f);
        }

        if (isHorizontalFacingChanged) {
            OrientAllObjects();
        }

        LookDirectionChanged?.Invoke(lookDirection);
    }

    private void OrientAllObjects() {
        foreach (Transform obj in objectsToOrient) {
            OrientObject(obj);
        }
    }

    private void OrientObject(Transform obj) {
        if (obj == null) { return; }

        if (lookDirection.x < 0) {
            obj.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else {
            obj.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void FlipLookOrientation() {
        SetLookDirection(-lookDirection);
    }
}