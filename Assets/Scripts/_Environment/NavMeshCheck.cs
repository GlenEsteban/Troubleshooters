using NavMeshPlus.Components;
using UnityEngine;

/// <summary>
/// Makeshift solution to enable NavMesh Surface component when game starts.
/// </summary>
public class NavMeshCheck : MonoBehaviour {
    private NavMeshSurface _navMeshSurface;

    private void Awake() {
        _navMeshSurface = GetComponent<NavMeshSurface>();

        _navMeshSurface.enabled = true;
    }
}