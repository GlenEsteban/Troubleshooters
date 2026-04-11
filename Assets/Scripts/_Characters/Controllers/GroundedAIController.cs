using UnityEngine;

/// <summary>
/// Controls grounded AI behavior by delegating updates to a behavior component
/// and enabling or disabling control based on interaction state.
/// </summary>
[RequireComponent(typeof(Rigidbody2DMovement))]
public class GroundedAIController : MonoBehaviour {
    [SerializeField] private GroundedAIBehavior currentBehavior;
    private Rigidbody2DMovement rigidBody2DMovement;
    private GrabbableObject grabbableObject;

    private bool hasControl = true;

    private void Awake() {
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
        grabbableObject = GetComponent<GrabbableObject>();

        // Check if behavior is a component of this game object
        if (currentBehavior != null && currentBehavior.gameObject != gameObject) {
            Debug.LogWarning("GroundedAIBehavior should usually be on the same GameObject.", this);
        }
    }

    private void OnEnable() {
        if (grabbableObject == null) { return; }

        grabbableObject.Grabbed += DisableControls;
        grabbableObject.Released += EnableControls;
    }
    private void OnDisable() {
        if (grabbableObject == null) { return; }

        grabbableObject.Grabbed -= DisableControls;
        grabbableObject.Released -= EnableControls;
    }
   
    private void Update() {
        if (!hasControl || currentBehavior == null) { return; }

        currentBehavior.UpdateBehavior();
    }

    public void EnableControls() {
        hasControl = true;

        rigidBody2DMovement.SetCanMove(true);
    }

    public void DisableControls() {
        hasControl = false;

        rigidBody2DMovement.HardStopMovement();
        rigidBody2DMovement.SetCanMove(false);
    }

    public void ChangeBehavior(GroundedAIBehavior behavior) {
        if (behavior == null || behavior == currentBehavior) { return; }

        if (behavior != null) {
            currentBehavior.enabled = false;
        }

        currentBehavior = behavior;
        behavior.enabled = true;
    }
}