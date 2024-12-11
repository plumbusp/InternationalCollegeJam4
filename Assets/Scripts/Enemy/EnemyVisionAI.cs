using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyVisionAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _enemyChaseSpeed;
    [SerializeField] private float _enemyNormalSpeed;
    [SerializeField] private float _deathRange;
    [SerializeField] private float PatrolStopDistance = 1f;
    [SerializeField] private float smooothRotationTime = 3f;

    [Header("References")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private HamsterMovement player;
    [SerializeField] private List<Transform> _wayPointsList;

    [Header("Mouse Settings & References")]
    [SerializeField] private Transform mouse;
    [SerializeField] private float _mouseEatingTime;
    private WaitForSeconds _eatMouse;

    private Queue<Transform> _wayPoints = new Queue<Transform>();

    private bool _isFollowingPlayer;
    private bool _isFollowingMouse = false;
    private bool _isWaiting = false;

    private bool _playerIsDead;
    private Vector3 _lastSeen;

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = _enemyNormalSpeed;

        fieldOfView.OnPlayerDetected += StartTargetFollowing;
        fieldOfView.OnMouseDetected += StartMouseFollowing;

        _eatMouse = new WaitForSeconds(_mouseEatingTime);

        foreach (var point in _wayPointsList)
        {
            if (point == null)
            {
                Debug.LogWarning("Waypoint cannot be null!");
                continue;
            }
            _wayPoints.Enqueue(point);
        }

        agent.SetDestination(GetNextPathPoint().position);
    }

    private void Update()
    {
        UpdateFieldOfView();
        if (_playerIsDead || _isWaiting)
            return;

        SmoothRotate();

        if(_isFollowingMouse)
        {
            Debug.Log("Following mouse");
            if (Vector2.Distance(transform.position, mouse.transform.position) <= _deathRange)
            {
                StartCoroutine(EatMouse());
            }
            agent.SetDestination(mouse.transform.position);
        }
        else if (_isFollowingPlayer)
        {
            HandleChasing();
        }
        else if (agent.remainingDistance <= PatrolStopDistance)
        {
            PatrolToNextPoint();
        }
    }

    private void StartTargetFollowing()
    {
        if (player.InSafeSpot || _playerIsDead)
            return;

        _isFollowingPlayer = true;
        agent.speed = _enemyChaseSpeed;
    }

    private void StartMouseFollowing()
    {
        player.InSafeSpot = true;
        _isFollowingMouse = true;
        agent.speed = _enemyChaseSpeed;
    }

    private void UpdateFieldOfView()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.right);
    }

    private void SmoothRotate()
    {
        //if (agent.velocity.sqrMagnitude > 0.01f) // Using sqrMagnitude for efficiency
        //{
        //    Vector2 direction = agent.velocity.normalized;
        //    float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //    float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * smooothRotationTime);
        //    transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        //}
        Vector2 direction = agent.velocity.normalized;
        transform.up = direction;
    }

    private void HandleChasing()
    {
        if (fieldOfView.IsTarget)
        {
            if (Vector2.Distance(transform.position, player.transform.position) <= _deathRange)
            {
                EatPlayer();
                return;
            }

            agent.SetDestination(player.transform.position);
            _lastSeen = player.transform.position;
        }
        else
        {
            StopChasing();
        }
    }

    private void PatrolToNextPoint()
    {
        agent.SetDestination(GetNextPathPoint().position);
    }

    private void EatPlayer()
    {
        agent.isStopped = true;
        _playerIsDead = true;
        Debug.Log("Player LOST!");
    }

    private void StopChasing()
    {
        _isFollowingPlayer = false;
        agent.speed = _enemyNormalSpeed;
        agent.SetDestination(_lastSeen);
    }

    private Transform GetNextPathPoint()
    {
        var next = _wayPoints.Dequeue();
        _wayPoints.Enqueue(next);
        return next;
    }

    private IEnumerator EatMouse()
    {
        _isWaiting = true;
        _isFollowingMouse = false;
        yield return _eatMouse;
        _isWaiting = false;
        player.InSafeSpot = false;
    }
}
