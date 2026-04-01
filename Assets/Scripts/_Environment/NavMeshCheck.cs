using NavMeshPlus.Components;
using UnityEngine;

public class NavMeshCheck : MonoBehaviour {
    private NavMeshSurface _navMeshSurface;

    private void Awake() {
        _navMeshSurface = GetComponent<NavMeshSurface>();

        _navMeshSurface.enabled = true;
    }
}