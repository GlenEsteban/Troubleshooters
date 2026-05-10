using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

/// <summary>
/// Applies spring-based forces to anchor points to simulate grabbing behavior,
/// and raises events when the object is grabbed or released.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class GrabbableObject : MonoBehaviour {
    public event Action Grabbed;
    public event Action Released;

    [Header("Weight")]
    [SerializeField, Range(0f, 9999f)] private float weight = 1000f;

    [Header("Distance Joint 2D")]
    [SerializeField, Range(0.01f, 20)] private float maxDistance = 0.05f;

    [Header("Hinge Joint 2D")]
    [SerializeField, Range(0f, 9999f)] private bool useLimits = true;
    [SerializeField, Range(-180, 0)] private float minLimit = -15;
    [SerializeField, Range(0, 180)] private float maxLimit = 15;

    [Header("Spring-Damper System")]
    [SerializeField, Range(0f, 5000f)] private float stiffness = 1000f;
    [SerializeField, Range(0f, 5000f)] private float damping = 30f;
    [SerializeField, Range(0f, 5000f)] private float maxForce = 1000f;

    private class AnchorPoint {
        public Vector2 anchorLocalPosition;
        public Vector2 targetWorldPosition;
    }

    private readonly Dictionary<int, AnchorPoint> anchorPoints = new Dictionary<int, AnchorPoint>();

    private Rigidbody2D rigidBody2D;

    public float GetDistributedWeight() {
        if (anchorPoints.Count == 0) { return weight; }

        return weight / anchorPoints.Count;
    }

    public Vector2 GetAnchorWorldPosition(int anchorId) {
        return transform.TransformPoint(anchorPoints[anchorId].anchorLocalPosition);
    }

    private void Awake() {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        //ApplyForcesAtAnchors();
    }

    private void ApplyForcesAtAnchors() {
        foreach (AnchorPoint anchorPoint in anchorPoints.Values) {
            Vector2 anchorWorldPosition = transform.TransformPoint(anchorPoint.anchorLocalPosition);
            Vector2 anchorVelocity = rigidBody2D.GetPointVelocity(anchorWorldPosition);
            Vector2 anchorToTargetDisplacement = anchorPoint.targetWorldPosition - anchorWorldPosition;

            Vector2 stiffnessForce = anchorToTargetDisplacement * stiffness;
            Vector2 dampingForce = - anchorVelocity * damping;

            Vector2 force = stiffnessForce + dampingForce;

            force = Vector2.ClampMagnitude(force, maxForce);

            rigidBody2D.AddForceAtPosition(force, anchorWorldPosition);
        }
    }

    public void AddAnchorPoint(int id, Vector2 worldPosition) {
        //if (anchorPoints.Count == 0) {
        //    Grabbed?.Invoke();
        //}

        //Vector2 localPosition = transform.InverseTransformPoint(worldPosition);

        //anchorPoints[id] = new AnchorPoint {
        //    anchorLocalPosition = localPosition,
        //    targetWorldPosition = worldPosition
        //};
    }

    public void UpdateAnchorTargetWorldPosition(int id, Vector2 worldPos) {
        //if (anchorPoints.TryGetValue(id, out AnchorPoint anchorPoint)) {
        //    anchorPoint.targetWorldPosition = worldPos;
        //}
    }

    public void RemoveAnchorPoint(int id) {
        //anchorPoints.Remove(id);

        //if (anchorPoints.Count == 0) {
        //    Released?.Invoke();
        //}
    }

    //public DistanceJoint2D CreateDistanceJoint2D(Rigidbody2D connectedRigidbody2D) {
    //    DistanceJoint2D distanceJoint2D = gameObject.AddComponent<DistanceJoint2D>();

    //    distanceJoint2D.connectedBody = connectedRigidbody2D;

    //    distanceJoint2D.distance = maxDistance;

    //    return distanceJoint2D;
    //}

    public HingeJoint2D CreateHingeJoint2D(Rigidbody2D connectedRigidbody2D, Vector3 anchorPosition) {
        HingeJoint2D hingeJoint2D = gameObject.AddComponent<HingeJoint2D>();

        if (hingeJoint2D == null) { return null; }

        hingeJoint2D.connectedBody = connectedRigidbody2D;

        hingeJoint2D.autoConfigureConnectedAnchor = false;
        hingeJoint2D.anchor = transform.InverseTransformPoint(anchorPosition);
        hingeJoint2D.connectedAnchor = Vector2.zero;

        hingeJoint2D.useLimits = useLimits;

        JointAngleLimits2D limits = hingeJoint2D.limits;

        limits.min = minLimit;
        limits.max = maxLimit;

        hingeJoint2D.limits = limits;

        return hingeJoint2D;
    }

    public void DestroyJoint(HingeJoint2D joint) {
        if (joint == null) { return; }

        Destroy(joint);
    }
}