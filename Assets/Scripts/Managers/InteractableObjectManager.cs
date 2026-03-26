using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectManager : MonoBehaviour {
    public static InteractableObjectManager Instance { get; private set; }

    [SerializeField] private List<GameObject> _interactableObjects= new List<GameObject>();
    public IReadOnlyList<GameObject> Items => _interactableObjects.AsReadOnly();

    public void Add(GameObject obj) {
        if (_interactableObjects.Contains(obj)) return;
        _interactableObjects.Add(obj);
    }
    public void Remove(GameObject obj) {
        _interactableObjects.Remove(obj);
    }
    public bool Contains(GameObject obj) {
        return _interactableObjects.Contains(obj);
    }

    private void Awake() {
        if (Instance != null &&  Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
    }
}