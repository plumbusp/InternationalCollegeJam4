using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class EnemyHearingAI : MonoBehaviour
{
    float smooothRotationTime = 3f;

    [SerializeField] private float _enemyChaseSpeed;
    [SerializeField] private float _enemyNormalSpeed;
    [SerializeField] private float _closeHearingRadius;

    //NavMesh
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float PatrolStopDistance = 1f;


    //Path 
    [Header("Path")]
    [SerializeField] private List<Transform> _wayPointsList;
    private Queue<Transform> _wayPoints = new Queue<Transform>();
    //private Vector3 nextPoint;

    //Sounds
    [Header("Sounds")]
    [SerializeField] private List<ISocialPlatform> _soundsMakers;

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = _enemyNormalSpeed;

        foreach (var t in _wayPointsList)
        {
            _wayPoints.Enqueue(t);
            if (t == null)
                Debug.LogWarning("Way Point cannot be null !!!! >:(");
        }

        foreach (ISoundMaker maker in _soundsMakers)
        {
            maker.OnLoudSoundMade += HandleLoudSound;
            maker.OnQuiteSoundMade += HandleQuiteSound;
        }
        agent.SetDestination(GetNextPathPoint().position);
    }

    private void Update()
    {
        SmoothRotate2D();

        if(agent.remainingDistance <= PatrolStopDistance)
        {
            agent.SetDestination(GetNextPathPoint().position);
        }
    }

    private void HandleLoudSound(Vector3 fromWhere)
    {
        agent.speed = _enemyChaseSpeed;
        agent.SetDestination(fromWhere);
    }
    private void HandleQuiteSound(Vector3 fromWhere)
    {
        Vector3 direction = fromWhere - transform.position;
        Quaternion.LookRotation(direction, Vector3.up);
        agent.speed = _enemyChaseSpeed;
        agent.SetDestination(fromWhere);
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
