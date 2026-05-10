using System.Collections;
using UnityEngine;

/// <summary>
/// Provides utility methods for managing ignored collision relationships
/// between composite objects.
/// </summary>
public static class IgnoredCollisionUtility {
    public static void IgnoreCollisions(Transform objectA, Transform objectB, bool isIgnored) {
        IgnoreCollisions(objectA, false, objectB, false, isIgnored);
    }

    public static void IgnoreCollisions(
        Transform objectA,
        bool useObjectATriggers,
        Transform objectB,
        bool useObjectBTriggers,
        bool isIgnored
    ) {
        Collider2D[] objectAColliders = objectA.GetComponentsInChildren<Collider2D>();
        Collider2D[] objectBColliders = objectB.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D colliderA in objectAColliders) {
            if (!useObjectATriggers && colliderA.isTrigger) { continue; }

            foreach (Collider2D colliderB in objectBColliders) {
                if (!useObjectBTriggers && colliderB.isTrigger) { continue; }

                Physics2D.IgnoreCollision(colliderA, colliderB, isIgnored);
            }
        }
    }

    public static bool CheckForOverlappingObjects(Transform objectA, Transform objectB, LayerMask targetLayer) {
        return CheckForOverlappingObjects(objectA, false, objectB, false, targetLayer);
    }

    public static bool CheckForOverlappingObjects(
        Transform objectA,
        bool useObjectATriggers,
        Transform objectB,
        bool useObjectBTriggers,
        LayerMask targetLayer
        ) {
        Collider2D[] objectAColliders = objectA.GetComponentsInChildren<Collider2D>();
        Collider2D[] objectBColliders = objectB.GetComponentsInChildren<Collider2D>();

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);

        Collider2D[] results = new Collider2D[10]; // Limit on results array size

        foreach (Collider2D colliderA in objectAColliders) {
            if (colliderA == null) { continue; }
            if (!useObjectATriggers && colliderA.isTrigger) { continue; }

            int overlapCount = colliderA.Overlap(filter, results);

            for (int i = 0; i < overlapCount; i++) {
                for (int j = 0; j < objectBColliders.Length; j++) {
                    if (objectBColliders[j] == null) { continue; }
                    if (!useObjectBTriggers && objectBColliders[j].isTrigger) { continue; }

                    if (results[i] == objectBColliders[j]) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static IEnumerator EnableCollisionsWhenSeparated(
        Transform objectA,
        Transform objectB,
        LayerMask targetLayer,
        float overlapCheckInterval
    ) {
        bool areObjectsOverlapping = true;
        WaitForSeconds wait = new WaitForSeconds(overlapCheckInterval);

        while (areObjectsOverlapping) {
            areObjectsOverlapping = CheckForOverlappingObjects(objectA, objectB, targetLayer);

            yield return wait;
        }

        IgnoreCollisions(objectA, objectB, false);
    }

    public static IEnumerator EnableCollisionsWhenSeparated(
        Transform objectA,
        bool useObjectATriggers,
        Transform objectB,
        bool useObjectBTriggers,
        LayerMask targetLayer,
        float overlapCheckInterval
    ) {
        bool areObjectsOverlapping = true;
        WaitForSeconds wait = new WaitForSeconds(overlapCheckInterval);

        while (areObjectsOverlapping) {
            areObjectsOverlapping = CheckForOverlappingObjects(objectA, useObjectATriggers, objectB, useObjectBTriggers, targetLayer);

            yield return wait;
        }

        IgnoreCollisions(objectA, useObjectATriggers, objectB, useObjectBTriggers, false);
    }
}