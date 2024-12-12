using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyVisionAI : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRotation;

    private float smooothRotationTime = 3f;

    [Header("Settings")]

    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float deathRange = 1.5f;

    [Header("References")]

    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private HamsterMovement player;
    private Transform currentTarget;


    private Vector3 lastKnownPosition;
    private bool isPlayerDead = false;

    private void Start()
    {
        startPos = transform.position;
        startRotation = transform.rotation;

        fieldOfView.OnPlayerDetected += HandlePlayerDetection;
    }

    private void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.forward);

        Destination();

        if (agent.remainingDistance <= .1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * smooothRotationTime);
    }

    private void Destination()
    {
        var destination = Vector3.zero;

        if(currentTarget != null)
        {
            if (fieldOfView.IsTarget)
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
        }
        else
        {
            destination = startPos;
            agent.stoppingDistance = 0;
        }

        agent.SetDestination(destination);
    }

    private void HandlePlayerDetection()
    {
        if (isPlayerDead) return;

        currentTarget = player.transform;
        agent.SetDestination(currentTarget.position);
        agent.speed = chaseSpeed;
    }
    private void StopChasingPlayer()
    {
        currentTarget = null;
        agent.speed = normalSpeed;
        agent.SetDestination(lastKnownPosition);
    }

    private void HandlePlayerCaught()
    {
        isPlayerDead = true;
        agent.isStopped = true;
        Debug.Log("Player LOST!");
    }

    private bool IsInRange(Transform target, float range)
    {
        return Vector2.Distance(transform.position, target.position) <= range;
    }
}
