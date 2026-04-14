using UnityEngine;

/// <summary>
/// Base class for AI behaviors that defines required components
/// and provides a common interface for updating behavior logic.
/// </summary>
public abstract class AIBehavior : MonoBehaviour {
    public virtual bool RequiresLookOrientation => false;
    public virtual bool RequiresRigidbody2DMovement => false;
    public virtual bool RequiresNavMeshRigidbody2DMovement => false;

    public abstract void UpdateBehavior();
}