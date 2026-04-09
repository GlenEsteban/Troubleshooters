using UnityEngine;

/// <summary>
/// Debug tool that enables mouse grab functionality.
/// </summary>
public class MouseGrabber : MonoBehaviour {
    public Camera cam;
    private GrabbableObject grabbedObject;

    private int pinchId = 0; 

    void Update() {
        // TEMP: INPUT FOR TESTING GRAB FUNCTIONALITY
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            TryGrab(mouseWorldPosition);
        }

        if (Input.GetMouseButton(0) && grabbedObject != null) {
            grabbedObject.UpdateAnchorTargetWorldPosition(pinchId, mouseWorldPosition);
        }

        if (Input.GetMouseButtonUp(0)) {
            Release();
        }
    }

    /// <summary>
    /// Checks for grabbable objects at the given world position and adds an anchor point.
    /// </summary>
    void TryGrab(Vector2 worldPos) {
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        if (hits.Length == 0) { return; }

        foreach (Collider2D hit in hits) {
            GrabbableObject grabbable = hit.GetComponentInParent<GrabbableObject>();

            if (grabbable == null) { continue; }

            grabbedObject = grabbable;
            grabbedObject.AddAnchorPoint(pinchId, worldPos);
        }
    }

    void Release() {
        if (grabbedObject == null) { return; }

        grabbedObject.RemoveAnchorPoint(pinchId);
        grabbedObject = null;        
    }
}