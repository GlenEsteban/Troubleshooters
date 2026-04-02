using System;
using UnityEngine;

/// <summary>
/// Detects valid collision impacts and breaks the object when the impact exceeds break threshold.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BreakableObject : MonoBehaviour {
    public event Action ObjectBroke;

    [SerializeField, Range(0, 100)] private float minImpactSpeedToBreak = 3f;
    [SerializeField] private LayerMask validImpactLayers;
    [SerializeField] private ParticleSystem shatterParticles;

    private bool isBroken;

    public bool IsBroken => isBroken;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (isBroken) { return; }

        if (!IsInLayerMask(collision.gameObject.layer, validImpactLayers)) { return; }

        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed >= minImpactSpeedToBreak) {
            Break();
        }
    }

    private bool IsInLayerMask(int layer, LayerMask mask) {
        // Convert the layer index into its bit representation.
        int layerBit = 1 << layer;

        // If the shared bit survives the AND operation, the layer exists in the mask.
        bool isInLayerMask = (mask.value & layerBit) != 0; 

        return isInLayerMask;
    }

    private void Break() {
        if (isBroken) { return; }

        isBroken = true;

        ObjectBroke?.Invoke();

        if (shatterParticles != null) {
            shatterParticles.Play();
        }
    }
}