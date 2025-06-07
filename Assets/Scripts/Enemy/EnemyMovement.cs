using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EnemyMovement;

public class EnemyMovement : MonoBehaviour
{
    public Action LookedAroundFoundNothing;

    [Header("Settings")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float deathRange = 1.5f;
    [SerializeField] private float patrolStopDistance = 1f;
    [SerializeField] private float smoothRotationSpeed = 5f;

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private List<Transform> waypoints;

    private Queue<Transform> waypointQueue;
    private Transform currentTarget;


    private EnemyParameters enemyParameters;
    private bool _isInitialized;

    // Look around parameters
    private bool lookAround;
    public void Intialize(EnemyParameters enemyParameters)
    {
        this.enemyParameters = enemyParameters;
        InitializeAgent();
        InitializeWaypoints();
        agent.updateRotation = false;
    }
    public enum MovementState
    {
        none,
        chasing,
        lookingAround
    }
    public MovementState movementState;

    private void Update()
    {
        SmoothRotateTowardsMovement();

        if (movementState == MovementState.none)
        {
            if (agent.remainingDistance < patrolStopDistance)
            {
                MoveToNextWaypoint();
                Debug.Log("Patroling");
            }
        }
    }

    public void Chase(Transform target)
    {
        movementState = MovementState.chasing;
        agent.SetDestination(target.position);
        Debug.Log("Chasing");
    }
    public void Patrol()
    {
        movementState = MovementState.none;
    }
    public void LookAround()
    {
        movementState = MovementState.lookingAround;
        //LookedAroundFoundNothing?.Invoke();
        Debug.Log("Looking around");
        movementState = MovementState.none;
    }

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

    #region Patrolling
    private void MoveToNextWaypoint()
    {
        var nextWaypoint = waypointQueue.Dequeue();
        waypointQueue.Enqueue(nextWaypoint);

        agent.SetDestination(nextWaypoint.position);
    }
    #endregion
    private void SmoothRotateTowardsMovement()
    {  
        if(agent.velocity.sqrMagnitude >= 0.01)
        {
            float currentAngle = transform.eulerAngles.z;
            float targetAngle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            float smoothLerp = Mathf.LerpAngle(targetAngle, currentAngle, Time.deltaTime * smoothRotationSpeed);
            transform.rotation = Quaternion.Euler(0, 0, smoothLerp);
        }
    }
}
