using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BreakableObject : MonoBehaviour {
    public event Action ObjectBroke;

    [SerializeField] private bool _isFragile = true;
    [SerializeField] private float _speedTillShatter = 3f;
    [SerializeField] private LayerMask _shatterLayers;


    private Rigidbody2D _rb;
    private Collider2D _collider;
    private ParticleSystem _shatterParticles;

    private bool _canShatter;
    private bool _isBroken;

    public void SetIsFragile(bool state) {
        _isFragile = state;
    }

    public bool GetIsBroken() {
        return _isBroken;
    }

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponentInChildren<Collider2D>();
        _shatterParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void Update() {
        if (_isBroken) return;

        if (_isFragile && _canShatter && _collider != null &&
            _collider.IsTouchingLayers(_shatterLayers)) {

            _isBroken = true;

            ObjectBroke?.Invoke();

            if (_shatterParticles != null) {
                _shatterParticles.Play();
            }
        }

        _canShatter = _rb.linearVelocity.magnitude > _speedTillShatter;
    }
}