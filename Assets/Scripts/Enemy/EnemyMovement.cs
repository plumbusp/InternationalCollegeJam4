using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
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

    private bool _allowedToMove;
    public bool isAllowedToMove
    {
        get
        {
            return _allowedToMove;
        }
        set
        {
            _allowedToMove = value;
            if(_allowedToMove == false)
                agent.isStopped = true;
            else
                agent.isStopped = false;
        }
    }

    public enum MovementState
    {
        none,
        chasing,
        lookingAround
    }
    public MovementState movementState;

    public void Intialize(EnemyParameters enemyParameters)
    {
        this.enemyParameters = enemyParameters;
        InitializeAgent();
        InitializeWaypoints();
        agent.updateRotation = false;

        //Handling no patrol situation
        if (waypoints.Count == 1)
        {
            noPatrol = true;
            initialPosition = waypoints[0].position;
            transform.position = initialPosition;
            initialEulerZ = transform.eulerAngles.z;
        }
        else if(waypoints.Count == 0)
        {
            noPatrol = true;
            initialPosition = transform.position;
            initialEulerZ = transform.eulerAngles.z;
        }
        //Handling no patrol situation
    }

    private void Update()
    {
        if(!_allowedToMove)
            return;

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
            else if (agent.remainingDistance < enemyParameters.patrolStopDistance)
            {
                MoveToNextWaypoint();
            }
        }
    }

    public void Chase(Transform target)
    {
        if (!_allowedToMove)
            return;

        Debug.Log("Chasing");
        movementState = MovementState.chasing;
        agent.SetDestination(target.position);
        agent.speed = enemyParameters.chaseSpeed;
    }
    public void Patrol()
    {
        if (!_allowedToMove)
            return;

        Debug.Log("Patrol");
        movementState = MovementState.none;
        if (noPatrol)
        {
            agent.SetDestination(initialPosition);
        }
        agent.speed = enemyParameters.normalSpeed;
    }
    public void LookAround()
    {
        if (!_allowedToMove)
            return;

        // Currently just goes to the last seen spot and waits for a bit
        Debug.Log("Looking around");
        movementState = MovementState.lookingAround;
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
        Patrol();
    }

    #region Initialization methods
    private void InitializeAgent()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = enemyParameters.normalSpeed;
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

    #endregion

    #region Patrolling
    private void MoveToNextWaypoint()
    {
        var nextWaypoint = waypointQueue.Dequeue();
        waypointQueue.Enqueue(nextWaypoint);

        agent.SetDestination(nextWaypoint.position);
    }
    #endregion

    #region Rotating
    private void SmoothRotateTowardsMovement()
    {  
        if(agent.velocity.sqrMagnitude >= 0.01)
        {
            float currentAngle = transform.eulerAngles.z;
            float targetAngle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            float smoothLerp = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * enemyParameters.smoothRotationSpeed);
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
            float t = Time.deltaTime * enemyParameters.smoothRotationSpeed;
            if (t < 0.05f) //Avoiding extremely small numbers
                t = 0.05f;
            float smoothLerp = Mathf.LerpAngle(currentAngle, initialEulerZ, t);
            transform.rotation = Quaternion.Euler(0, 0, smoothLerp);
            //Debug.Log($"{initialEulerZ} initialEulerZ and {currentAngle} currentAngle  and {smoothLerp} smoothLerp");
        }
    }
    #endregion
}
