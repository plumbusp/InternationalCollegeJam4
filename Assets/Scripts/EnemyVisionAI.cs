using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVisionAI : MonoBehaviour
{
    Quaternion startRotation;

    float smooothRotationTime = 3f;

    [SerializeField] float _enemyChaseSpeed;
    [SerializeField] float _enemyNormalSpeed;
    [SerializeField] FieldOfView fieldOfView;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] float PlayerStopDistance = 1f;
    [SerializeField] float PatrolStopDistance = 1f;


    //Path 
    [SerializeField] private List<Transform> _wayPointsList;
    private Queue<Transform> _wayPoints = new Queue<Transform>();
    private Transform nextPoint;
    //Chasing
    private bool _isFollowingTarget;

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
    }

    private void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.right);

        Destination();
        SmoothRotate2D();
        //if (agent.remainingDistance <= .1f)
        //    transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * smooothRotationTime);
    }

    private void Destination()
    {
        if (_isFollowingTarget)
        {
            if(agent.remainingDistance <= PatrolStopDistance)
            {
                _isFollowingTarget = false;
            }
            return;
        }

        var destination = Vector3.zero;

        if (nextPoint == null || Vector2.Distance(transform.position, nextPoint.position) <= PatrolStopDistance)
        {
            nextPoint = GetNextPathPoint();
            agent.speed = _enemyNormalSpeed;
            destination = nextPoint.position;
        }
        else
        {
            destination = nextPoint.position;
        }

        agent.SetDestination(destination);
    }

    private void StartTargetFollowing()
    {
        _isFollowingTarget = true;
        nextPoint = null;
        agent.speed = _enemyChaseSpeed;
        agent.SetDestination(target.position);
    }

    private void SmoothRotate2D()
    {
        if (agent.velocity.magnitude > 0.1f) // Only rotate if the agent is moving
        {
            Vector2 direction = agent.velocity.normalized; // Calculate the movement direction

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;             // Calculate the target angle based on direction

            float currentAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * smooothRotationTime);             // Smoothly rotate towards the target angle
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            //SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();             // Optional: Flip the sprite based on direction
            //if (spriteRenderer != null)
            //{
            //    spriteRenderer.flipX = direction.x < 0; // Flip horizontally if moving left
            //}
        }
    }

    private Transform GetNextPathPoint()
    {
        Debug.Log("GetNext POINTTT");
        var nextPoint = _wayPoints.Dequeue();
        _wayPoints.Enqueue(nextPoint);
        return nextPoint;
    }
}

//if (fieldOfView.IsTarget)
//{
//    nextPoint = null;
//    _lastSeen = target.position;
//    destination = target.position;
//    agent.stoppingDistance = PlayerStopDistance;
//    agent.speed = _enemyChaseSpeed;
//}
//else if(!fieldOfView.IsTarget && !_checkedLastSeen)
//{
//    destination = _lastSeen;
//}
//else if(!fieldOfView.IsTarget && !_checkedLastSeen && Vector2.Distance(transform.position, _lastSeen) <= PatrolStopDistance)
//{
//    _checkedLastSeen = true;
//}
//else
//{
//    Debug.Log();
//}