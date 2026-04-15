using UnityEngine;

/// <summary>
/// Controls a grounded simple patrol AI behavior that moves along the ground 
/// and turns around when reaching an edge or hitting an obstacle.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
public class GrabbedAIBehavior : AIBehavior {
    public override bool RequiresLookOrientation => true;

    [Header("Ground Detection")]
    [SerializeField] private Collider2D groundDetection;
    [SerializeField] private LayerMask groundLayers;

    private LookOrientation lookOrientation;

    private bool wasGrounded;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
    }

    private void Start() {
        wasGrounded = CheckIfGrounded();
    }

    public override void UpdateBehavior() {
        if (!CheckIfGrounded()) {
            FlipOnLiftOff();

            wasGrounded = false;


            return;
        }

        wasGrounded = true;
    }

    private bool CheckIfGrounded() {
        if (groundDetection == null) { return false; }

        return groundDetection.IsTouchingLayers(groundLayers);
    }

    private void FlipOnLiftOff() {
        if (wasGrounded) {
            lookOrientation.FlipLookOrientation();
        }
    }
}