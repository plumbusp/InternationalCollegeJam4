using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private Coroutine currentCoroutine;

    private bool noPatrol;
    private float initialEulerZ;
    private Vector3 initialPosition;

    public void Intialize(EnemyParameters enemyParameters)
    {
        this.enemyParameters = enemyParameters;
        InitializeAgent();
        InitializeWaypoints();
        agent.updateRotation = false;
        if (waypoints.Count == 1)
        {
            noPatrol = true;
        }
        initialEulerZ = transform.eulerAngles.z;
        initialPosition = transform.position;

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
            if (noPatrol)
            {
                if(Vector3.Distance(agent.transform.position, initialPosition) < 2f)
                {
                    agent.transform.position = initialPosition;
                    SmoothRotateToInitialRotation();
                }
            }
            else if (agent.remainingDistance < patrolStopDistance)
            {
                MoveToNextWaypoint();
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
        Debug.Log("Patrol");
        movementState = MovementState.none;
        if (noPatrol)
        {
            agent.SetDestination(initialPosition);
        }
    }
    public void LookAround()
    {
        // Currently just goes to the last seen spot and waits for a bit
        movementState = MovementState.lookingAround;
        Debug.Log("Looking around");
        if(currentCoroutine != null)
        {
            Debug.LogWarning("The coroutine is already running. Restarting the coroutine.");
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(LookAroundCoroutine());
    }

    private IEnumerator LookAroundCoroutine()
    {
        yield return new WaitForSeconds(3f);
        LookedAroundFoundNothing?.Invoke();
        Patrol();
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
    private void SmoothRotateToInitialRotation()
    {
        if (transform.eulerAngles.z == initialEulerZ)
        {
            return;
        }
        else if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, initialEulerZ)) < 3f)
        {
            transform.rotation = Quaternion.Euler(0, 0, initialEulerZ);
            transform.position = initialPosition;
            return;
        }
        else
        {
            float currentAngle = transform.eulerAngles.z;
            float t = Time.deltaTime * smoothRotationSpeed;
            if (t < 0.05f) //Avoiding extremely small numbers
                t = 0.05f;
            float smoothLerp = Mathf.LerpAngle(currentAngle, initialEulerZ, t);
            transform.rotation = Quaternion.Euler(0, 0, smoothLerp);
            //Debug.Log($"{initialEulerZ} initialEulerZ and {currentAngle} currentAngle  and {smoothLerp} smoothLerp");
        }
    }
}
