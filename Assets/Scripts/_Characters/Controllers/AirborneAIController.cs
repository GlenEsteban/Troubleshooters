using System;
using System.Collections.Generic;
using UnityEngine;

public class AirborneAIController : MonoBehaviour {
    [Header("General")]
    [SerializeField] private AirborneBehavior _airborneBehavior;

    [Header("Simple Patrol Behavior")]
    [SerializeField] private bool _hasFrontObstacleDetection = true;
    [SerializeField] private bool _hasCeilingObstacleDetection = true;
    [SerializeField] private bool _hasGroundObstacleDetection = true;
    [SerializeField] private bool _hasHardStopOnObstacleDetection = true;    
    [SerializeField] private LayerMask _obstacleLayers;
    [SerializeField] private Collider2D _horizontalObstacleDetector;
    [SerializeField] private Collider2D _ceilingObstacleDetector;
    [SerializeField] private Collider2D _groundObstacleDetector;
    [SerializeField] private Vector2 _startingMoveDirection = Vector2.right;
    [SerializeField] private float _detectionRate = 0.1f;

    [Header("Advance Patrol Behavior")]
    [SerializeField] private List<Transform> _patrolPoints;
    [Range(0f, 30f), SerializeField] private float _waitTimeAtPatrolPoint = 3f;
    [Range(0f, 10f), SerializeField] private float _stoppingDistance = 1.5f;

    [Header("Follow Target Behavior")]
    [SerializeField] private Transform _target;

    private LookOrientation _lookOrientation;
    private Rigidbody2DMovement _rigidBody2DMovement;
    private NavMeshRigidbody2DMovement _navMeshRigidbody2DMovement;
    private GrabbableObject _grabbableObject;

    private float _detectionTimer;
    private float _waitAtPatrolPointTimer;

    public Transform _targetPatrolPoint;
    private int _targetPatrolPointIndex;


    private void Awake() {
        _lookOrientation = GetComponent<LookOrientation>();
        _rigidBody2DMovement = GetComponent<Rigidbody2DMovement>();
        _navMeshRigidbody2DMovement = GetComponent<NavMeshRigidbody2DMovement>();
        _grabbableObject = GetComponent<GrabbableObject>();
    }
    private void OnDisable() {
        if (_grabbableObject == null) { return; }

        _grabbableObject.OnGrab -= DisableControls;
        _grabbableObject.OnDrop -= EnableControls;
    }
    void Start() {
        // Subscribe controller access to grabbable object events
        if (_grabbableObject != null) {
            _grabbableObject.OnGrab += DisableControls;
            _grabbableObject.OnDrop += EnableControls;
        }

        // Set transform as default patrol point if there are none
        if (_patrolPoints.Count == 0) {
            _patrolPoints.Add(transform);
        }

        // Set first patrol point as target patrol point
        _targetPatrolPoint = _patrolPoints[0];
        _targetPatrolPointIndex = 0;
    }

    private void Update() {
        switch (_airborneBehavior) {
            case AirborneBehavior.None:
                _rigidBody2DMovement.MoveInDirection(Vector2.zero);
                break;
            case AirborneBehavior.SimplePatrol:
                SimplePatrolBehavior();
                break;
            case AirborneBehavior.AdvancePatrol:
                AdvancePatrolBehviour();
                break;
            case AirborneBehavior.FollowTarget:
                FollowTargetBehavior();
                break;
        }
    }

    private void SimplePatrolBehavior() {
        _navMeshRigidbody2DMovement.StopMovement();

        if (_rigidBody2DMovement.GetMoveDirection() == Vector2.zero ) {
            _rigidBody2DMovement.MoveInDirection( _startingMoveDirection);
        }

        _detectionTimer += Time.deltaTime;
        if (_detectionTimer > _detectionRate) {
            ReflectMoveDirectionAtObstacle();

            _detectionTimer = 0;
        }
    }

    private void ReflectMoveDirectionAtObstacle() {
        if (_hasFrontObstacleDetection && _horizontalObstacleDetector.IsTouchingLayers(_obstacleLayers)) {
            if (_hasHardStopOnObstacleDetection) {
                _rigidBody2DMovement.HardStopMovement();
            }

            Vector2 currentMoveDirection = _rigidBody2DMovement.GetMoveDirection();
            Vector2 reflectedMoveDirection = new Vector2(-currentMoveDirection.x, currentMoveDirection.y);

            _rigidBody2DMovement.MoveInDirection(reflectedMoveDirection);
            _lookOrientation.SetLookOrientation(reflectedMoveDirection);
        }

        if (_hasCeilingObstacleDetection && _ceilingObstacleDetector.IsTouchingLayers(_obstacleLayers)) {
            if (_hasHardStopOnObstacleDetection) {
                _rigidBody2DMovement.HardStopMovement();
            }

            Vector2 currentMoveDirection = _rigidBody2DMovement.GetMoveDirection();
            Vector2 reflectedMoveDirection = new Vector2 (currentMoveDirection.x, - Math.Abs(currentMoveDirection.y));

            _rigidBody2DMovement.MoveInDirection(reflectedMoveDirection);
            _lookOrientation.SetLookOrientation(reflectedMoveDirection);
        }  

        if (_hasGroundObstacleDetection && _groundObstacleDetector.IsTouchingLayers(_obstacleLayers)) {
            if (_hasHardStopOnObstacleDetection) {
                _rigidBody2DMovement.HardStopMovement();
            }

            Vector2 currentMoveDirection = _rigidBody2DMovement.GetMoveDirection();
            Vector2 reflectedMoveDirection = new Vector2(currentMoveDirection.x, Math.Abs(currentMoveDirection.y));

            _rigidBody2DMovement.MoveInDirection(reflectedMoveDirection);
            _lookOrientation.SetLookOrientation(reflectedMoveDirection);
        }
    }

    private void AdvancePatrolBehviour() {
        _rigidBody2DMovement.StopMovement();
         
        float distanceFromPatrolPoint = Vector2.Distance((Vector2) _targetPatrolPoint.position, (Vector2) transform.position);
        if (distanceFromPatrolPoint <= _stoppingDistance) {
            _waitAtPatrolPointTimer += Time.deltaTime;

            if (_waitAtPatrolPointTimer > _waitTimeAtPatrolPoint) {
                if (_targetPatrolPointIndex < _patrolPoints.Count - 1) {
                    _targetPatrolPointIndex++;
                    _targetPatrolPoint = _patrolPoints[_targetPatrolPointIndex];
                }
                else {
                    _targetPatrolPointIndex = 0;
                    _targetPatrolPoint = _patrolPoints[0];
                }

                _waitAtPatrolPointTimer = 0;
            }
        }

        _navMeshRigidbody2DMovement.MoveToTarget(_targetPatrolPoint.position);
        _lookOrientation.SetLookOrientation(_navMeshRigidbody2DMovement.GetCurrentMoveDirection());
    }

    private void FollowTargetBehavior() {
        _rigidBody2DMovement.StopMovement();

        _navMeshRigidbody2DMovement.MoveToTarget(_target.position);

        Vector2 selfToTargetDirection = (_target.position - transform.position).normalized;
        _lookOrientation.SetLookOrientation(selfToTargetDirection);
    }

    public void EnableControls() {
        _rigidBody2DMovement.SetCanMove(true);
        _navMeshRigidbody2DMovement.SetCanMove(true);
    }

    public void DisableControls() {
        _rigidBody2DMovement.HardStopMovement();
        _rigidBody2DMovement.SetCanMove(false);
        _navMeshRigidbody2DMovement.SetCanMove(false);
    }
}

public enum AirborneBehavior {
    None, 
    SimplePatrol,
    AdvancePatrol,
    FollowTarget
}