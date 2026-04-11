using UnityEngine;

/// <summary>
/// Base class for a grounded AI behavior that depend on ground detection
/// and expose a behavior update method.
/// </summary>
public abstract class GroundedAIBehavior : MonoBehaviour {
    [SerializeField] protected Collider2D groundDetection;
    [SerializeField] protected LayerMask groundLayers;

    protected virtual bool CheckIfGrounded() {
        if (groundDetection == null) { return false; }

        return groundDetection.IsTouchingLayers(groundLayers);
    }

    public abstract void UpdateBehavior();
}