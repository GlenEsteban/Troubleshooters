using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central manager responsible for tracking and classifying all interactable objects.
/// Other systems should query this class instead of storing direct references
/// to multiple interactable objects.
/// </summary>
public class InteractableObjectManager : MonoBehaviour {
    public static InteractableObjectManager Instance { get; private set; }

    [SerializeField] private List<GameObject> interactableObjects= new List<GameObject>();

    public IReadOnlyList<GameObject> InteractableObjects => interactableObjects.AsReadOnly();

    private void Awake() {
        // Ensure only one instance exists
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Registers an object with the manager.
    /// </summary>
    public void RegisterObject(GameObject obj) {
        if (!interactableObjects.Contains(obj)) {
            interactableObjects.Add(obj);
        }
    }

    /// <summary>
    /// Unregisters an object from the manager.
    /// </summary>
    public void UnregisterObject(GameObject obj) {
        if (interactableObjects.Contains(obj)) {
            interactableObjects.Remove(obj);
        }
    }
}