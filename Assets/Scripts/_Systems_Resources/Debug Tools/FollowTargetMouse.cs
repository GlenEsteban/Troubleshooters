using UnityEngine;

/// <summary>
/// Debug tool that moves this object to follow the mouse cursor in world space.
/// Useful for testing target-following behavior.
/// </summary>
public class FollowTargetMouse : MonoBehaviour {
    [SerializeField] private Camera cam;
    void Update() {
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);

        // Reset Z to 0 for 2D
        mouseWorldPosition.z = 0;

        transform.position = mouseWorldPosition;
    }
}