using System;
using System.Collections.Generic;
using UnityEngine;

public class ClawAttachment : MonoBehaviour {
    public event Action AccidentalDrop;

    [SerializeField] private int _anchorId;
    [SerializeField] private Transform _anchorPoint;

    [SerializeField] private float _accidentalDropDistance = 0.8f;

    [SerializeField] private List<GrabbableObject> _grabbableObjectsInPickupCollider = new List<GrabbableObject>();
    [SerializeField] private GrabbableObject _targetGrabbableObject;

    private void OnTriggerEnter2D(Collider2D collision) {
        GrabbableObject grabbableObject = collision.GetComponentInParent<GrabbableObject>();
        if (grabbableObject == null) { return; }

        if (!_grabbableObjectsInPickupCollider.Contains(grabbableObject)) {
            _grabbableObjectsInPickupCollider.Add(grabbableObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        GrabbableObject grabbableObject = collision.GetComponentInParent<GrabbableObject>();
        if (grabbableObject == null) { return; }
        
        if (_grabbableObjectsInPickupCollider.Contains(grabbableObject)) {
            _grabbableObjectsInPickupCollider.Remove(grabbableObject);
        }
    }

    private void Update() {
        if (_targetGrabbableObject != null) {
            _targetGrabbableObject.UpdateAnchorPointPosition(_anchorId, _anchorPoint.position);

            Vector2 targetLocalAnchorPoint = _targetGrabbableObject.GetWorldAnchorPoint(_anchorId);
            float targetToAnchorPointDistance = ((Vector3)targetLocalAnchorPoint - _anchorPoint.position).magnitude;
            if (targetToAnchorPointDistance > _accidentalDropDistance) {
                AccidentalDrop?.Invoke();
                Drop();

                print("accidental drop");
            }
        }
    }

    public void Grab() {
        if (_targetGrabbableObject == null && _grabbableObjectsInPickupCollider.Count > 0) {
            _targetGrabbableObject = _grabbableObjectsInPickupCollider[0];
            _targetGrabbableObject.AddAnchorPoint(_anchorId, _anchorPoint.position);

        }
    }

    public void Drop() {
        if (_targetGrabbableObject != null) {
            _targetGrabbableObject.RemoveAnchorPoint(_anchorId);
            _targetGrabbableObject = null;
        }
    }
}