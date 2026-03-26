using System;
using UnityEngine;

public class LookOrientation : MonoBehaviour {
    public event Action<Vector2> OrientationChanged;

    [SerializeField] private Transform _detectors;
    [SerializeField] private Transform _attachments;


    private Vector2 _lookOrientation = Vector2.right;

    public void SetLookOrientation(Vector2 direction) {
        if (_lookOrientation == direction) { return; }

        _lookOrientation = direction;

        OrientationChanged?.Invoke(_lookOrientation);

        OrientateObject(_detectors);
        OrientateObject(_attachments);
    }

    public Vector2 GetLookOrientation() {
        return _lookOrientation;
    }

    private void OrientateObject(Transform obj) {
        if (obj == null) { return; }

        if (_lookOrientation.x == 0) return;

        if (_lookOrientation.x < 0) {
            obj.rotation = Quaternion.Euler(0, 180, 0);
        }
        else {
            obj.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}