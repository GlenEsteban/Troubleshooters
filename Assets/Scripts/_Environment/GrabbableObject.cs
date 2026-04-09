using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies spring-based forces to anchor points to simulate grabbing behavior,
/// and raises events when the object is grabbed or released.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class GrabbableObject : MonoBehaviour {
    public event Action Grabbed;
    public event Action Released;

    [SerializeField] private float stiffness = 1000f;
    [SerializeField] private float damping = 30f;
    [SerializeField] private float maxForce = 1000f;

    private class AnchorPoint {
        public Vector2 anchorLocalPosition;
        public Vector2 targetWorldPosition;
    }

    private readonly Dictionary<int, AnchorPoint> anchorPoints = new Dictionary<int, AnchorPoint>();

    private Rigidbody2D rb;

    public Vector2 GetAnchorWorldPosition(int anchorId) {
        return transform.TransformPoint(anchorPoints[anchorId].anchorLocalPosition);
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        ApplyForcesAtAnchors();
    }

    /// <summary>
    /// Applies spring and damping forces at each anchor point,
    /// pulling them toward their target positions while reducing oscillation.
    /// </summary>
    private void ApplyForcesAtAnchors() {
        foreach (AnchorPoint anchorPoint in anchorPoints.Values) {
            Vector2 anchorWorldPosition = transform.TransformPoint(anchorPoint.anchorLocalPosition);
            Vector2 anchorVelocity = rb.GetPointVelocity(anchorWorldPosition);
            Vector2 anchorToTargetDisplacement = anchorPoint.targetWorldPosition - anchorWorldPosition;

            Vector2 stiffnessForce = anchorToTargetDisplacement * stiffness;
            Vector2 dampingForce = anchorVelocity * damping;

            Vector2 force = stiffnessForce - dampingForce;

            force = Vector2.ClampMagnitude(force, maxForce);

            rb.AddForceAtPosition(force, anchorWorldPosition);
        }
    }

    public void AddAnchorPoint(int id, Vector2 worldPosition) {
        if (anchorPoints.Count == 0) {
            Grabbed?.Invoke();
        }

        Vector2 localPosition = transform.InverseTransformPoint(worldPosition);

        anchorPoints[id] = new AnchorPoint {
            anchorLocalPosition = localPosition,
            targetWorldPosition = worldPosition
        };
    }

    public void UpdateAnchorTargetWorldPosition(int id, Vector2 worldPos) {
        if (anchorPoints.TryGetValue(id, out AnchorPoint anchorPoint)) {
            anchorPoint.targetWorldPosition = worldPos;
        }
    }

    public void RemoveAnchorPoint(int id) {
        anchorPoints.Remove(id);

        if (anchorPoints.Count == 0) {
            Released?.Invoke();
        }
    }
}