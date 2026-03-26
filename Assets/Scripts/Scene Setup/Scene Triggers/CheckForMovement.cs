using UnityEngine;

public class CheckForMovement : MonoBehaviour {
    [SerializeField] Rigidbody2D _playerRB;

    private bool _hasMoved = false;
    public void HasMoved(bool state) {
        _hasMoved = state;
    }

    private void Update() {
        if (!_hasMoved && _playerRB.linearVelocity.magnitude > 2f) {
            _hasMoved = true;
        }
    }
}