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
    private bool followingTarget = false;
    private bool checkingLastSeen;

    private Vector3 intialPosition;
    private Vector3 _lastSeen;


    private void Start()
    {
        fieldOfView.OnPlayerDetected += HandlePlyerDetection;
    }

    private void Update()
    {
        SmoothRotateTowardsMovement();

        if (!followingTarget)
        {
            return;
        }

        if (followingTarget && fieldOfView.IsTarget)
        {
            if (Vector2.Distance(transform.position, player.transform.position) <= deathRange)
            {
                agent.isStopped = true;
                Debug.Log("Player is dead");
            }
            agent.SetDestination(player.transform.position);
            _lastSeen = player.transform.position;
        }
        else if(followingTarget && !fieldOfView.IsTarget)
        {
            followingTarget = false;
            checkingLastSeen = true;
            agent.SetDestination(_lastSeen);
            agent.speed = normalSpeed;
        }
        else if (checkingLastSeen)
        {
            if(agent.remainingDistance <= patrolStopDistance)
            {
                checkingLastSeen = false;
                agent.SetDestination(intialPosition);
            }
        }
    }
    private void HandlePlyerDetection()
    {
        followingTarget = true;
        agent.speed = chaseSpeed;
    }

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
