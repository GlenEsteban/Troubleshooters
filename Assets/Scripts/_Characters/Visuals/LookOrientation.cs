using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls left/right orientation by horizontally flipping assigned child transforms
/// based on look direction.
/// 
/// Do not assign transforms that are driven by physics joints, as flipping them can
/// interfere with joint behavior.
/// </summary>
public class LookOrientation : MonoBehaviour {
    public event Action<Vector2> LookDirectionChanged;
    public Vector2 LookDirection => lookDirection;

    [SerializeField] private List<Transform> objectsToOrient;

    private Vector2 lookDirection = Vector2.right;

    public void SetLookDirection(Vector2 direction) {
        if (Mathf.Approximately(direction.x, 0f)) return;
        if (Mathf.Approximately(lookDirection.x, direction.x)) return;

        lookDirection = direction;

        foreach (Transform obj in objectsToOrient) {
            if (obj == null) { continue; }

            OrientObject(obj);
        }

        LookDirectionChanged?.Invoke(direction);
    }

    private void OrientObject(Transform obj) {
        if (obj == null) { return; }

        if (lookDirection.x == 0) return;

        if (lookDirection.x < 0) {
            obj.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else {
            obj.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}