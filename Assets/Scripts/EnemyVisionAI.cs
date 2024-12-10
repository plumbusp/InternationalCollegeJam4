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
    private bool reachedPreviousPoint = true;

    private void Start()
    {
        startRotation = transform.rotation;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = _enemyNormalSpeed;

        foreach (var t in _wayPointsList)
        {
            _wayPoints.Enqueue(t);
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
        var destination = Vector3.zero;

        if (fieldOfView.IsTarget)
        {
            nextPoint = null;
            destination = target.position;
            agent.stoppingDistance = PlayerStopDistance;
            agent.speed = _enemyChaseSpeed;
        }
        else if(nextPoint == null || Vector2.Distance(transform.position, nextPoint.position) <= PatrolStopDistance)
        {
            nextPoint = GetNextPathPoint();
            //Debug.Log("Next Position : " + nextPoint);
            if (nextPoint == null)
            {
                Debug.LogWarning("Way Point cannot be null !!!! >:(");
            }
            destination = nextPoint.position;
            agent.speed = _enemyNormalSpeed;
        }
        else
        {
            destination = nextPoint.position;
        }

        agent.SetDestination(destination);
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
