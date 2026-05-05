using UnityEngine;

/// <summary>
/// Enables or disables collision between two sets of colliders.
/// </summary>
public class ObjectCollisionPhasing : MonoBehaviour {
    [SerializeField] private bool useTriggers = false;
    public void SetIgnoredCollision(Collider2D[] objectAColliders, Collider2D[] objectBColliders, bool ignore) {
        foreach (Collider2D colliderA in objectAColliders) {
            if (colliderA == null) { continue; }
            if (!useTriggers && colliderA.isTrigger) { continue; }

            foreach (Collider2D colliderB in objectBColliders) {
                if (colliderB == null) { continue; }
                if (!useTriggers && colliderB.isTrigger) { continue; }

                Physics2D.IgnoreCollision(colliderA, colliderB, ignore);
            }
        }
    }
}