using UnityEngine;

public abstract class AIBehavior : MonoBehaviour {
    public virtual bool RequiresLookOrientation => false;
    public virtual bool RequiresRigidbody2DMovement => false;
    public virtual bool RequiresNavMeshRigidbody2DMovement => false;

    public abstract void UpdateBehavior();
}
