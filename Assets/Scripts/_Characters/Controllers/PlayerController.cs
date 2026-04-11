using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    private PlayerInput _playerInput;
    private Rigidbody2DMovement _playerMovement;
    private ClawAttachment _clawAttachment;
    private LookOrientation _lookOrientation;

    private void Awake() {
        _playerInput = new PlayerInput();
        _lookOrientation = GetComponent<LookOrientation>();
        _playerMovement = GetComponent<Rigidbody2DMovement>();

        _clawAttachment = GetComponentInChildren<ClawAttachment>();
    }

    private void OnEnable() {
        _playerInput.Enable();

        _playerInput.Player.Move.performed += Move;
        _playerInput.Player.Move.canceled += Move;
        _playerInput.Player.GrabDrop.performed += GrabDrop;
        _playerInput.Player.GrabDrop.canceled += GrabDrop;
    }

    private void OnDisable() {
        _playerInput.Disable();

        _playerInput.Player.Move.performed -= Move;
        _playerInput.Player.Move.canceled -= Move;
        _playerInput.Player.GrabDrop.performed -= GrabDrop;
        _playerInput.Player.GrabDrop.canceled -= GrabDrop;
    }

    private void Move(InputAction.CallbackContext context) {
        Vector2 moveDirection = context.ReadValue<Vector2>();

        _playerMovement.SetMoveDirection(moveDirection);
        _lookOrientation.SetLookDirection(moveDirection);
    }

    private void GrabDrop(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            _clawAttachment.Use();
        }
    }
}