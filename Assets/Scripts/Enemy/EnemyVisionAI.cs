using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVisionAI : MonoBehaviour
{
    Quaternion startRotation;

    float smooothRotationTime = 3f;

    [SerializeField] private float _enemyChaseSpeed;
    [SerializeField] private float _enemyNormalSpeed;
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private HamsterMovement target;
    [SerializeField] private float PatrolStopDistance = 1f;


    //Path 
    [SerializeField] private List<Transform> _wayPointsList;
    private Queue<Transform> _wayPoints = new Queue<Transform>();
    private Transform nextPoint;

    //Chasing
    [SerializeField] private float _deathRange;
    private bool _isFollowingTarget;
    private Vector3 _lastSeen;

    private Action OnTargetReached;
    
    private void Start()
    {
        startRotation = transform.rotation;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = _enemyNormalSpeed;

        fieldOfView.OnPlayerDetected += StartTargetFollowing;

        foreach (var t in _wayPointsList)
        {
            _wayPoints.Enqueue(t);
            if (t == null)
                Debug.LogWarning("Way Point cannot be null !!!! >:(");
        }
        nextPoint = GetNextPathPoint();
        agent.SetDestination(nextPoint.position);
    }

    private void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.right);

        SmoothRotate2D();

        if (_isFollowingTarget)
        {
            //Active chasing
            if (fieldOfView.IsTarget)
            {
                if(Vector2.Distance(transform.position, target.transform.position) <= _deathRange)
                {
                    agent.isStopped = true;
                    Debug.Log("Player LOST!");
                    return;
                }
                agent.SetDestination(target.transform.position);
                _lastSeen = target.transform.position;
            }
            //check the point
            else
            {
                _isFollowingTarget = false; //breaking from following "if"
                Debug.Log("Broke from if, last seen:" + _lastSeen);
                agent.speed = _enemyNormalSpeed;
                agent.SetDestination(_lastSeen);
            }
        }
        
        else if(!fieldOfView.IsTarget && agent.remainingDistance <= PatrolStopDistance)
        {
            nextPoint = GetNextPathPoint();
            agent.SetDestination(nextPoint.position);
        }
    }

    private void StartTargetFollowing()
    {
        if (target.InSafeSpot)
            return;
        _isFollowingTarget = true;
        agent.speed = _enemyChaseSpeed;
    }

    private void SmoothRotate2D()
    {
        if (agent.velocity.magnitude > 0.1f) 
        {
            Vector2 direction = agent.velocity.normalized; 

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            float currentAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * smooothRotationTime); 
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }

    private Transform GetNextPathPoint()
    {
        var nextPoint = _wayPoints.Dequeue();
        _wayPoints.Enqueue(nextPoint);
        return nextPoint;
    }
}