using NavMeshPlus.Components;
using UnityEngine;

/// <summary>
/// Makeshift solution to enable NavMesh Surface component when game starts.
/// </summary>
public class NavMeshCheck : MonoBehaviour {
    private NavMeshSurface navMeshSurface;

    private void Awake() {
        navMeshSurface = GetComponent<NavMeshSurface>();

        navMeshSurface.enabled = true;
    }
}