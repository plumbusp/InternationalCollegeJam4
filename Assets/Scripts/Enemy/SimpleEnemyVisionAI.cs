using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyVisionAI : MonoBehaviour
{
    Vector3 startPos;
    float startRotationZ;

    float smooothRotationTime = 3f;

    [SerializeField] FieldOfView fieldOfView;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] float stoppingDistance = 1f;
    [SerializeField] float agentSpeed;

    private void Start()
    {
        agent.updateRotation = false; 
        agent.updateUpAxis = false;

        startPos = transform.position;
        startRotationZ = transform.rotation.z;
        agent.speed = agentSpeed;
    }

    private void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.forward);

        Destination();

        if (agent.remainingDistance <= .1f)
        {
            float newZRotation = Mathf.LerpAngle(transform.eulerAngles.z, startRotationZ, Time.deltaTime * smooothRotationTime);
            transform.rotation = Quaternion.Euler(0, 0, newZRotation);
        }
    }

    private void Destination()
    {
        var destination = Vector3.zero;

        if (fieldOfView.IsTarget)
        {
            destination = target.position;
            agent.stoppingDistance = stoppingDistance;
        }
        else
        {
            destination = startPos;
            agent.stoppingDistance = 0;
        }

        agent.SetDestination(destination);
    }

}
