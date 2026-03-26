using UnityEngine;

public class GroundedAIController : MonoBehaviour {
    [Header("Targeting")]
    [SerializeField] private bool _isFollowingTarget;
    [SerializeField] private Transform _target;
    [SerializeField] private float _targetingRatePerSec = 0.1f;
    [SerializeField] private float _followingDistance= 1f;

    [Header("Collider Detection")]
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private LayerMask _obstacleLayers;
    [SerializeField] private bool _hasEdgeDetection = true;
    [SerializeField] private bool _hasHardStopOnEdgeDetection = true;
    [SerializeField] private bool _hasObstacleDetection = true;
    [SerializeField] private bool _hasHardStopOnObstacleDetection = true;    
    [SerializeField] private float _detectionRatePerSec = 0.1f;
    [SerializeField] private Collider2D _obstacleDetection;
    [SerializeField] private Collider2D _groundDetection;
    [SerializeField] private Collider2D _edgeDetection;

    private Rigidbody2DMovement _rigidBody2DMovement;
    private LookOrientation _lookOrientation;

    private float _targetingTimer;
    private float _detectionTimer;

    private bool _isGrounded;

    private Vector2 _moveDirection;

    private void Awake() {
        _lookOrientation = GetComponent<LookOrientation>();
        _rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
    }

    private void Start() {
        _moveDirection = Vector2.right;
        _rigidBody2DMovement.MoveInDirection(_moveDirection);
    }

    private void Update() {
        HandleGroundDetectionBehavior();

        _targetingTimer += Time.deltaTime;
        if (_isFollowingTarget && _targetingTimer > _targetingRatePerSec) {
            HandleFollowingBehavior();
        }

        if (_isGrounded) {
            _detectionTimer += Time.deltaTime;
        }
        if (!_isFollowingTarget && _detectionTimer > _detectionRatePerSec) {
            HandleEdgeDetectionBehavior();
            HandleObstacleDetectionBehavior();
        }
    }

    private void HandleFollowingBehavior() {
        float signedHorizontalDistanceToPlayer = _target.position.x - this.transform.position.x;
        float horizontalDistanceToPlayer = Mathf.Abs(signedHorizontalDistanceToPlayer);

        if (horizontalDistanceToPlayer > _followingDistance) {
            _moveDirection = signedHorizontalDistanceToPlayer > 0 ? Vector2.right : Vector2.left;
            _rigidBody2DMovement.MoveInDirection(_moveDirection);
            _lookOrientation.SetLookOrientation(_moveDirection);
        }
        _targetingTimer = 0;
    }

    private void HandleGroundDetectionBehavior() {
        if (!_groundDetection.IsTouchingLayers(_groundLayers)) {
            _rigidBody2DMovement.SetCanMove(false);
            _isGrounded = false;
        }
        else {
            _rigidBody2DMovement.SetCanMove(true);
            _isGrounded = true;
        }
    }

    private void HandleEdgeDetectionBehavior() {
        if (_hasEdgeDetection && !_edgeDetection.IsTouchingLayers(_groundLayers)) {
            if (_hasHardStopOnEdgeDetection) {
                _rigidBody2DMovement.HardStopMovement();
            }

            Vector2 newDirection = -_rigidBody2DMovement.GetMoveDirection();
            _rigidBody2DMovement.FlipHorizontalDirection();
            _lookOrientation.SetLookOrientation(newDirection);

            _detectionTimer = 0;
        }
    }

    private void HandleObstacleDetectionBehavior() {
        if (_hasObstacleDetection && _obstacleDetection.IsTouchingLayers(_obstacleLayers)) {

            if (_hasHardStopOnObstacleDetection) {
                _rigidBody2DMovement.HardStopMovement();
            }

            Vector2 newDirection = -_rigidBody2DMovement.GetMoveDirection();
            _rigidBody2DMovement.MoveInDirection(newDirection);
            _lookOrientation.SetLookOrientation(newDirection);

            _detectionTimer = 0;
        }
    }
}