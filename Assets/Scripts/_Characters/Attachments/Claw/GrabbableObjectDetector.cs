using System.Collections.Generic;
using UnityEngine;

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

    public void RemoveDestroyedObjects() {
        objectsInRange.RemoveAll(item => item == null);
    }
}
