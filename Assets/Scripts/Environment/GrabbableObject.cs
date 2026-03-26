using System;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour {
    public event Action OnGrab;
    public event Action OnDrop;

    [SerializeField] private bool _isGrabbable = true;
    [SerializeField] private float stiffness = 1000f;
    [SerializeField] private float damping = 30f;
    [SerializeField] private float maxForce = 1000f;

    private Rigidbody2D rb;

    private Dictionary<int, AnchorPoint> anchorPoints = new Dictionary<int, AnchorPoint>();

    public bool IsGrabbable => _isGrabbable;
    public void SetIsGrabbable(bool state) {
        _isGrabbable = state;
    }
    public Vector2 GetWorldAnchorPoint(int anchorId) {
        return transform.TransformPoint(anchorPoints[anchorId].localAnchor);
    }

    private void Start() {
        InteractableObjectManager.Instance.Add(this.gameObject);
    }

    private void OnDisable() {
        InteractableObjectManager.Instance.Remove(this.gameObject);
    }
    class AnchorPoint {
        public Vector2 localAnchor;
        public Vector2 target;
    }

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void AddAnchorPoint(int id, Vector2 worldPos) {
        if (anchorPoints.Count == 0) {
            OnGrab?.Invoke();
        }

        Vector2 localPosition = transform.InverseTransformPoint(worldPos);

        anchorPoints[id] = new AnchorPoint {
            localAnchor = localPosition,
            target = worldPos
        };
    }

    public void UpdateAnchorPointPosition(int id, Vector2 worldPos) {
        if (anchorPoints.ContainsKey(id)) {
            anchorPoints[id].target = worldPos;
        }
    }

    public void RemoveAnchorPoint(int id) {
        if (anchorPoints.ContainsKey(id)) {
            anchorPoints.Remove(id);
        }

        if (anchorPoints.Count == 0) {
            OnDrop?.Invoke();
        }
    }

    void FixedUpdate() {
        if (!_isGrabbable) { return; }

        foreach (var anchorPoint in anchorPoints.Values) {
            Vector2 worldAnchor = transform.TransformPoint(anchorPoint.localAnchor);

            Vector2 pointVelocity = rb.GetPointVelocity(worldAnchor);
            Vector2 directiontToTarget = anchorPoint.target - worldAnchor;

            Vector2 pullTowardTarget = directiontToTarget * stiffness;
            Vector2 resistanceToMotion = pointVelocity * damping;

            Vector2 force = pullTowardTarget - resistanceToMotion;

            force = Vector2.ClampMagnitude(force, maxForce);

            rb.AddForceAtPosition(force, worldAnchor);
        }
    }
}