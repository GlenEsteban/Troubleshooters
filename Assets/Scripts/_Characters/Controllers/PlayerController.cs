using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Processes player input to control movement, orientation,
/// and attachment interactions.
/// </summary>

[RequireComponent(typeof(Rigidbody2DMovement))]
[RequireComponent(typeof(LookOrientation))]
public class PlayerController : MonoBehaviour {
    private PlayerInputActions playerInputActions;
    private Rigidbody2DMovement rigidBody2DMovement;
    private LookOrientation lookOrientation;

    private ClawAttachment clawAttachment;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        lookOrientation = GetComponent<LookOrientation>();
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();

        // TEMP: will add multiple attachments and an attachmentController 
        clawAttachment = GetComponentInChildren<ClawAttachment>();
    }

    private void OnEnable() {
        playerInputActions.Enable();

        playerInputActions.Player.Move.performed += Move;
        playerInputActions.Player.Move.canceled += Move;
        playerInputActions.Player.AttachmentPrimaryUse.performed += AttachmentPrimaryUse;
        playerInputActions.Player.AttachmentSecondaryUse.performed += AttachmentSecondaryUse;
    }

    private void OnDisable() {
        playerInputActions.Disable();

        playerInputActions.Player.Move.performed -= Move;
        playerInputActions.Player.Move.canceled -= Move;
        playerInputActions.Player.AttachmentPrimaryUse.performed -= AttachmentPrimaryUse;
        playerInputActions.Player.AttachmentSecondaryUse.performed -= AttachmentSecondaryUse;
    }

    private void Move(InputAction.CallbackContext context) {
        Vector2 moveDirection = context.ReadValue<Vector2>();

        rigidBody2DMovement.SetMoveDirection(moveDirection);
        lookOrientation.SetLookDirection(moveDirection);
    }

    private void AttachmentPrimaryUse(InputAction.CallbackContext context) {
        clawAttachment?.PrimaryUse();
    }

    private void AttachmentSecondaryUse(InputAction.CallbackContext context) {
        clawAttachment.SecondaryUse();
    }
}