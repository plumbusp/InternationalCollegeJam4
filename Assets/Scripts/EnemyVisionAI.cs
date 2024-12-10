using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVisionAI : MonoBehaviour
{
    Quaternion startRotation;

    float smooothRotationTime = 3f;

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

        foreach(var t in _wayPointsList)
        {
            _wayPoints.Enqueue(t);
        }
    }

    private void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.forward);

        Destination();

        //if (agent.remainingDistance <= .1f)
        //    transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * smooothRotationTime);
    }

    private void Destination()
    {
        var destination = Vector3.zero;

        if (fieldOfView.IsTarget)
        {
            reachedPreviousPoint = true;
            destination = target.position;
            agent.stoppingDistance = PlayerStopDistance;
        }
        
        else
        {
            if (reachedPreviousPoint)
            {
                nextPoint = GetNextPathPoint();
                Debug.Log("Next Position : " + nextPoint);
                if (nextPoint == null)
                {
                    Debug.LogWarning("Way Point cannot be null !!!! >:(");
                }
                agent.destination = nextPoint.position;
            }
            else if(Vector2.Distance(transform.position, nextPoint.position) <= PatrolStopDistance) 
            {
                reachedPreviousPoint = true;
                Debug.Log(Vector2.Distance(transform.position, nextPoint.position));
            }
        }

           
        //if (!reachedPreviousPoint)
        //{
        //    reachedPreviousPoint = false;
        //    nextPoint = GetNextPathPoint();
        //    destination = nextPoint.position;
        //    Debug.Log("Next Position : " + nextPoint);
        //}

        //Debug.Log("Distance: " + Vector2.Distance(transform.position, nextPoint.position));
        //if (Vector2.Distance(transform.position, nextPoint.position) <= PatrolStopDistance)
        //{
        //    reachedPreviousPoint = true;
        //    Debug.Log("REACHEEED");
        //}

        //agent.SetDestination(destination);
    }

    private Transform GetNextPathPoint()
    {
        Debug.Log("GetNext POINTTT");
        var nextPoint = _wayPoints.Dequeue();
        _wayPoints.Enqueue(nextPoint);
        return nextPoint;
    }
}
