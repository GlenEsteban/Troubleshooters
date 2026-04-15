using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enables or disables control based on interaction state, delegates AI behavior updates
/// to the current behavior, and configures components required by the current behavior.
/// </summary>
public class AIController : MonoBehaviour {
    [SerializeField] protected AIBehavior grabbedBehavior;
    [SerializeField] protected AIBehavior currentBehavior;

    private LookOrientation lookOrientation;
    private Rigidbody2DMovement rigidBody2DMovement;
    private NavMeshRigidbody2DMovement navMeshRigidbody2DMovement;
    private GrabbableObject grabbableObject;

    private bool isGrabbed = false;

    private AIBehavior previousBehavior;

    private void Awake() {
        // Optional components. Used only if required by the current behavior.
        lookOrientation = GetComponent<LookOrientation>();
        rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
        navMeshRigidbody2DMovement = GetComponent<NavMeshRigidbody2DMovement>();

        // Not required, but affects control state if present.
        grabbableObject = GetComponent<GrabbableObject>();
    }

    private void Start() {
        DisableAllBehaviorComponents();

        if (currentBehavior == null) { return; }

        InitializeCurrentBehaviorComponent();
    }

    private void OnEnable() {
        if (grabbableObject == null) { return; }

        grabbableObject.Grabbed += SwitchToGrabbedAIBehavior;
        grabbableObject.Released += SwitchBackToPreviousBehavior;
    }

    private void OnDisable() {
        if (grabbableObject == null) { return; }

        grabbableObject.Grabbed -= SwitchToGrabbedAIBehavior;
        grabbableObject.Released -= SwitchBackToPreviousBehavior;
    }

    private void Update() {
        if (currentBehavior == null) { return; }

        currentBehavior.UpdateBehavior();
    }
    private void DisableAllBehaviorComponents() {
        AIBehavior[] behaviors = GetComponents<AIBehavior>();

        foreach (AIBehavior behavior in behaviors) {
            behavior.enabled = behavior == currentBehavior;
        }
    }

    private void InitializeCurrentBehaviorComponent() {
        ConfigureRequiredComponentsForBehavior();

        currentBehavior.enabled = true;
    }

    public void EnableControls() {
        rigidBody2DMovement.SetCanMove(true);
        navMeshRigidbody2DMovement.SetCanMove(true);
    }

    public void DisableControls() {
        rigidBody2DMovement.SetCanMove(false);
        navMeshRigidbody2DMovement.SetCanMove(false);
    }

    private void SwitchToGrabbedAIBehavior() {
        if (isGrabbed) { return; }

        isGrabbed = true;

        previousBehavior = currentBehavior;

        DisableControls();

        ChangeBehavior(grabbedBehavior);
    }

    private void SwitchBackToPreviousBehavior() {
        if (!isGrabbed) { return; }

        isGrabbed = false;

        EnableControls();

        ChangeBehavior(previousBehavior);
    }

    public void ChangeBehavior(AIBehavior behavior) {
        if (behavior == null || behavior == currentBehavior) { return; }

        if (currentBehavior != null) {
            currentBehavior.enabled = false;
        }

        currentBehavior = behavior;

        ConfigureRequiredComponentsForBehavior();

        currentBehavior.enabled = true;
    }

    private void ConfigureRequiredComponentsForBehavior() {
        if (currentBehavior == null) { return; }

        if (lookOrientation != null) {
            lookOrientation.enabled = currentBehavior.RequiresLookOrientation;
        }

        if (rigidBody2DMovement != null) {
            rigidBody2DMovement.enabled = currentBehavior.RequiresRigidbody2DMovement;
        }

        if (navMeshRigidbody2DMovement != null) {
            navMeshRigidbody2DMovement.enabled = currentBehavior.RequiresNavMeshRigidbody2DMovement;
        }
    }
}