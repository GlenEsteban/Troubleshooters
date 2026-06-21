using System;
using UnityEngine;

/// <summary>
/// Creates, configures, and destroys hinge joint 2D on grabbable object.
/// </summary>
public class GrabbableObject : MonoBehaviour {
    public event Action Grabbed;
    public event Action Released;

    [Header("Weight")]
    [SerializeField, Range(0f, 9999f)] private float weight = 1000f;

    [Header("Hinge Joint 2D")]
    [SerializeField, Range(0f, 9999f)] private bool useLimits = true;
    [SerializeField, Range(-180, 0)] private float minLimit = -180;
    [SerializeField, Range(0, 180)] private float maxLimit = 180;

    public float anchorPointCount = 0;

    public float GetDistributedWeight() {
        if (anchorPointCount == 0) { return weight; }

        return weight / anchorPointCount;
    }


    public HingeJoint2D CreateHingeJoint2D(Rigidbody2D connectedRigidbody2D, Vector3 anchorPosition) {
        HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();

        if (joint == null) { return null; }

        joint.connectedBody = connectedRigidbody2D;

        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = transform.InverseTransformPoint(anchorPosition);
        joint.connectedAnchor = Vector2.zero;

        joint.useLimits = useLimits;

        JointAngleLimits2D limits = joint.limits;

        limits.min = minLimit;
        limits.max = maxLimit;

        joint.limits = limits;

        if (anchorPointCount <= 0) {
            Grabbed?.Invoke();
        }

        anchorPointCount++;

        return joint;
    }

    public void DestroyHingeJoint2D(HingeJoint2D joint) {
        if (joint == null) { return; }

        Destroy(joint);

        anchorPointCount--;

        if (anchorPointCount <= 0) {
            Released?.Invoke();
        }
    }
}