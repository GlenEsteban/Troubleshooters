using UnityEngine;

/// <summary>
/// Controls an AI behavior while the character is grabbed, flipping its look
/// orientation once when it is lifted off the ground.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
public class GrabbedAIBehaviorFlipOnLiftOff : AIBehavior {
    public override bool RequiresLookOrientation => true;

    [Header("Ground Detection")]
    [SerializeField] private Collider2D groundDetection;
    [SerializeField] private LayerMask groundLayers;

    private LookOrientation lookOrientation;

    private bool wasGrounded;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
    }

    private void OnEnable() {
        wasGrounded = CheckIfGrounded();
    }

    public override void UpdateBehavior() {
        bool isGrounded = CheckIfGrounded();

        if (!isGrounded && wasGrounded) {
            lookOrientation.FlipLookOrientation();
        }

        wasGrounded = isGrounded;
    }

    private bool CheckIfGrounded() {
        if (groundDetection == null) { return false; }

        return groundDetection.IsTouchingLayers(groundLayers);
    }
}