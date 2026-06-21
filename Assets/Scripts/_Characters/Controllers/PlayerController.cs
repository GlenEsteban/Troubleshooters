using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Processes player input to control movement, orientation,
/// and attachment interactions across different devices.
/// </summary>
[RequireComponent(typeof(Rigidbody2DMovement))]
[RequireComponent(typeof(LookOrientation))]
public class PlayerController : MonoBehaviour {
    [SerializeField] private Transform playerCenter;

    private PlayerInputActions playerInputActions;
    private InputDevice currentDevice;
    private Rigidbody2DMovement rigidBody2DMovement;
    private LookOrientation lookOrientation;

    private ClawAttachment clawAttachment;

    private Vector2 interactPointMoveDirection;

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
        playerInputActions.Player.SwitchClawMode.performed += SwitchClawMode;
        playerInputActions.Player.SwitchClawMode.canceled += SwitchClawMode;
        playerInputActions.Player.MoveInteractPoint.performed += MoveInteractPoint;
        playerInputActions.Player.MoveInteractPoint.canceled += MoveInteractPoint;
        playerInputActions.Player.AttachmentPrimaryUse.performed += AttachmentPrimaryUse;
        playerInputActions.Player.AttachmentSecondaryUse.performed += AttachmentSecondaryUse;
    }

    private void OnDisable() {
        playerInputActions.Disable();

        playerInputActions.Player.Move.performed -= Move;
        playerInputActions.Player.Move.canceled -= Move;
        playerInputActions.Player.SwitchClawMode.performed -= SwitchClawMode;
        playerInputActions.Player.SwitchClawMode.canceled -= SwitchClawMode;
        playerInputActions.Player.MoveInteractPoint.performed -= MoveInteractPoint;
        playerInputActions.Player.MoveInteractPoint.canceled -= MoveInteractPoint;
        playerInputActions.Player.AttachmentPrimaryUse.performed -= AttachmentPrimaryUse;
        playerInputActions.Player.AttachmentSecondaryUse.performed -= AttachmentSecondaryUse;
    }

    private void Update() {
        UpdateClawInteractPointPosition();

        UpdateClawInteractPointMoveDirection();
    }

    private void UpdateClawInteractPointPosition() {
        if (currentDevice is Mouse || currentDevice is Keyboard) {
            clawAttachment.SetIsMovingInteractPoint(false);

            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            Vector2 selfToMouseDisplacement = mouseWorldPosition - (Vector2)playerCenter.transform.position;

            lookOrientation.SetLookDirection(selfToMouseDisplacement.normalized);

            clawAttachment.SetInteractPointPosition(mouseWorldPosition);
        }
    }

    private void UpdateClawInteractPointMoveDirection() {
        if (currentDevice is Gamepad) {
            clawAttachment.SetInteractPointMoveDirection(interactPointMoveDirection);
        }
    }

    private void Move(InputAction.CallbackContext context) {
        if (context.performed) {
            currentDevice = context.control.device;
        }

        if (currentDevice is Gamepad) {
            Vector2 lookDirection = context.ReadValue<Vector2>();

            lookOrientation.SetLookDirection(lookDirection);
        }

        Vector2 moveDirection = context.ReadValue<Vector2>();

        rigidBody2DMovement.SetMoveDirection(moveDirection);
    }

    private void SwitchClawMode(InputAction.CallbackContext context) {
        if (context.performed) {
            currentDevice = context.control.device;

            clawAttachment.ToggleInteractMode();
        }
    }

    private void MoveInteractPoint(InputAction.CallbackContext context) {
        if (context.performed && currentDevice is Gamepad) {
            currentDevice = context.control.device;

            clawAttachment.SetIsMovingInteractPoint(true);
        }

        interactPointMoveDirection = context.ReadValue<Vector2>();
    }

    private void AttachmentPrimaryUse(InputAction.CallbackContext context) {
        if (context.performed) {
            currentDevice = context.control.device;

            clawAttachment.PrimaryUse();
        }
    }

    private void AttachmentSecondaryUse(InputAction.CallbackContext context) {
        if (context.performed) {
            currentDevice = context.control.device;

            clawAttachment.SecondaryUse();
        }
    }
}