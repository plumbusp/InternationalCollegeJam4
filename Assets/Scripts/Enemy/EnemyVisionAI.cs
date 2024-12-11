using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyVisionAI : MonoBehaviour
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

    [Header("Mouse Settings")]
    [SerializeField] private Mouse mouse;
    [SerializeField] private float mouseEatingTime = 3f;

    private WaitForSeconds mouseEatingDelay;
    private Queue<Transform> waypointQueue;
    private Transform currentTarget;

    private bool isPlayerDead = false;
    private bool isWaiting = false;
    private Vector3 lastKnownPosition;

    private Vector2 _distance;

    private void Start()
    {
        InitializeAgent();
        InitializeWaypoints();
        SubscribeToEvents();

        mouseEatingDelay = new WaitForSeconds(mouseEatingTime);

        MoveToNextWaypoint();
    }

    private void Update()
    {
        UpdateFieldOfView();
        if (isPlayerDead || isWaiting) return;
        SmoothRotateTowardsMovement();

        if (currentTarget != null)
        {
            ProcessTargetBehavior();
        }
        else if (agent.remainingDistance <= patrolStopDistance)
        {
            MoveToNextWaypoint();
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
        fieldOfView.OnMouseDetected += HandleMouseDetection;
    }
    #endregion

    #region Field of View
    private void UpdateFieldOfView()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.right);
    }
    #endregion

    #region Target Handling
    private void HandlePlayerDetection()
    {
        if (player.InSafeSpot || isPlayerDead) return;

        SetTarget(player.transform, chaseSpeed);
    }

    private void HandleMouseDetection()
    {
        player.InSafeSpot = true;
        SetTarget(mouse.transform, chaseSpeed);
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
        }
        else if (currentTarget == mouse.transform)
        {
            Debug.Log(IsInRange(mouse.transform, deathRange) + "   " + Vector2.Distance(transform.position, currentTarget.position));
            if (IsInRange(mouse.transform, deathRange))
            {
                StartCoroutine(HandleMouseInteraction());
            }
        }
    }
    #endregion

    #region Target Interaction
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

    private IEnumerator HandleMouseInteraction()
    {
        isWaiting = true;
        currentTarget = null;

        yield return mouseEatingDelay;

        isWaiting = false;
        player.InSafeSpot = false;
        mouse.Eat();
        MoveToNextWaypoint();
    }
    #endregion

    #region Patrolling
    private void MoveToNextWaypoint()
    {
        var nextWaypoint = waypointQueue.Dequeue();
        waypointQueue.Enqueue(nextWaypoint);

        agent.SetDestination(nextWaypoint.position);
    }
    #endregion

    #region Rotation
    private void SmoothRotateTowardsMovement()
    {
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 direction = agent.velocity.normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * smoothRotationSpeed);
            transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        }
    }
}
    #endregion