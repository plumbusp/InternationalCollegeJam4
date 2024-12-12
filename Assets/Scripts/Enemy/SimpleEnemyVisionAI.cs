using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyVisionAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float deathRange = 1.5f;
    [SerializeField] private float patrolStopDistance = 1f;
    [SerializeField] private float smoothRotationSpeed = 5f;

    [Header("References")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private HamsterMovement player;
    [SerializeField] private List<Transform> waypoints;

    private Vector3 _initialPosition;


    private Queue<Transform> waypointQueue;
    private Transform currentTarget;

    private bool isPlayerDead = false;
    private bool isWaiting = false;
    private Vector3 lastKnownPosition;

    private void Start()
    {
        InitializeAgent();
        InitializeWaypoints();
        SubscribeToEvents();

        _initialPosition = transform.position;
    }

    private void Update()
    {
        UpdateFieldOfView();
        if (isPlayerDead || isWaiting) return;

        if (currentTarget != null)
        {
            ProcessTargetBehavior();
        }
        else if (_initialPosition.magnitude <= patrolStopDistance)
        {
            agent.SetDestination(_initialPosition);
        }
    }

    #region Initialization
    private void InitializeAgent()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = normalSpeed;
    }

    private void InitializeWaypoints()
    {
        waypointQueue = new Queue<Transform>();
        foreach (var waypoint in waypoints)
        {
            if (waypoint != null)
            {
                waypointQueue.Enqueue(waypoint);
            }
            else
            {
                Debug.LogWarning("Waypoint is null! Skipping...");
            }
        }
    }

    private void SubscribeToEvents()
    {
        fieldOfView.OnPlayerDetected += HandlePlayerDetection;
    }
    #endregion

    #region Field of View
    private void UpdateFieldOfView()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(-transform.up);
    }
    #endregion

    private void HandlePlayerDetection()
    {
        if (player.InSafeSpot || isPlayerDead) return;

        SetTarget(player.transform, chaseSpeed);
    }

    private void SetTarget(Transform target, float speed)
    {
        currentTarget = target;
        agent.speed = speed;
    }

    private void ProcessTargetBehavior()
    {
        agent.SetDestination(currentTarget.position);

        if (currentTarget == player.transform)
        {
            if (IsInRange(player.transform, deathRange))
            {
                HandlePlayerCaught();
            }
            else if (!fieldOfView.IsTarget)
            {
                StopChasingPlayer();
            }

            lastKnownPosition = player.transform.position;
        }
    }

    private bool IsInRange(Transform target, float range)
    {
        return Vector2.Distance(transform.position, target.position) <= range;
    }

    private void HandlePlayerCaught()
    {
        isPlayerDead = true;
        agent.isStopped = true;
        Debug.Log("Player LOST!");
    }

    private void StopChasingPlayer()
    {
        currentTarget = null;
        agent.speed = normalSpeed;
        agent.SetDestination(lastKnownPosition);
    }
}