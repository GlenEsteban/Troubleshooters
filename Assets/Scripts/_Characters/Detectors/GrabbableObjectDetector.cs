using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks nearby GrabbableObjects via trigger events, maintaining a read-only list
/// of valid objects in range while excluding itself and cleaning up destroyed references.
/// </summary>
public class GrabbableObjectDetector : MonoBehaviour {
    public IReadOnlyList<GrabbableObject> ObjectsInRange => objectsInRange;

    private List<GrabbableObject> objectsInRange = new List<GrabbableObject>();

    private GrabbableObject self;

    private void Awake() {
        self = GetComponentInParent<GrabbableObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        RegisterGrabbableObjects(collision);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        UnregisterGrabbableObject(collision);
    }

    private void RegisterGrabbableObjects(Collider2D collision) {
        GrabbableObject grabbableObject = collision.GetComponentInParent<GrabbableObject>();

        if (grabbableObject == null) { return; }
        if (grabbableObject == self) { return; }

        if (!objectsInRange.Contains(grabbableObject)) {
            objectsInRange.Add(grabbableObject);
        }
    }

    private void UnregisterGrabbableObject(Collider2D collision) {
        GrabbableObject grabbableObject = collision.GetComponentInParent<GrabbableObject>();

        if (grabbableObject == null) { return; }
        if (grabbableObject == self) { return; }

        if (objectsInRange.Contains(grabbableObject)) {
            objectsInRange.Remove(grabbableObject);
        }
    }

    private void RemoveDestroyedObjects() {
        objectsInRange.RemoveAll(item => item == null);
    }

    public GrabbableObject GetFirstGrabbableObject() {
        RemoveDestroyedObjects();

        if (ObjectsInRange.Count <= 0) { return null; }

        return objectsInRange[0];
    }
}
