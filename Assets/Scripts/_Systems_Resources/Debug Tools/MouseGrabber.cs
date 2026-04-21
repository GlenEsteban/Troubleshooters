using UnityEngine;

/// <summary>
/// Debug tool that enables mouse grab functionality.
/// </summary>
public class MouseGrabber : MonoBehaviour {
    [SerializeField] private Camera cam;

    private GrabbableObject grabbedObject;

    private int anchorID = -99; 

    void Update() {
        // TEMP: INPUT FOR TESTING GRAB FUNCTIONALITY
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            TryGrab(mouseWorldPosition);
        }

        if (Input.GetMouseButton(0) && grabbedObject != null) {
            grabbedObject.UpdateAnchorTargetWorldPosition(anchorID, mouseWorldPosition);
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
            grabbedObject.AddAnchorPoint(anchorID, worldPos);
        }
    }

    void Release() {
        if (grabbedObject == null) { return; }

        grabbedObject.RemoveAnchorPoint(anchorID);
        grabbedObject = null;        
    }
}