using UnityEngine;

public class MouseGrabber : MonoBehaviour {
    public Camera cam;

    private GrabbableObject grabbedObject;
    private int pinchId = 0; // single mouse = one pinch

    void Update() {
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            TryGrab(mouseWorld);
        }

        if (Input.GetMouseButton(0) && grabbedObject != null) {
            grabbedObject.UpdateAnchorPointPosition(pinchId, mouseWorld);
        }

        if (Input.GetMouseButtonUp(0)) {
            Release();
        }
    }

    void TryGrab(Vector2 worldPos) {
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);

        if (hits.Length == 0) { return; }

        foreach (RaycastHit2D hit in hits) {
            var grabbable = hit.collider.GetComponentInParent<GrabbableObject>();

            if (grabbable != null) {
                grabbedObject = grabbable;
                grabbedObject.AddAnchorPoint(pinchId, worldPos);
            }
        }
    }

    void Release() {
        if (grabbedObject != null) {
            grabbedObject.RemoveAnchorPoint(pinchId);
            grabbedObject = null;
        }
    }
}