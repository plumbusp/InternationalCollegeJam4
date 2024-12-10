using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVisionAI : MonoBehaviour
{
    [SerializeField] private float normalRotationRate = 25f;
    [SerializeField] private float chaseRotationRate = 50f;

    [SerializeField] private float _enemyChaseSpeed;
    [SerializeField] private float _enemyNormalSpeed;
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    [SerializeField] private float PlayerDeadDistance;
    [SerializeField] private float PatrolStopDistance = 1f;


    //Path 
    [SerializeField] private List<Transform> _wayPointsList;
    private Queue<Transform> _wayPoints = new Queue<Transform>();
    private Transform nextPoint;
    private bool _playerIsDead = false;
    private bool _isChasingPlayer;
    private Vector3 _lastPLayerPosition;

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = _enemyNormalSpeed;

        foreach (var t in _wayPointsList)
        {
            _wayPoints.Enqueue(t);
        }

        nextPoint = GetNextPathPoint();
    }

    private void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.right);

        if (_playerIsDead)
            return;

        if (fieldOfView.IsTarget && !_isChasingPlayer)
        {
            _isChasingPlayer = true;
        }

        Destination();
        RapidRotate2d();
    }

    private void Destination()
    {
        if (_isChasingPlayer)
        {
            ChasePlayer();
            return;
        }

        var destination = Vector3.zero;
        if(nextPoint == null || Vector2.Distance(transform.position, nextPoint.position) <= PatrolStopDistance)
        {
            nextPoint = GetNextPathPoint();
            if (nextPoint == null)
                Debug.LogWarning("Way Point cannot be null !!!! >:(");
            destination = nextPoint.position;
            agent.speed = _enemyNormalSpeed;
        }
        else
        {
            destination = nextPoint.position;
        }

        agent.SetDestination(destination);
    }

    private void ChasePlayer()
    {
        // Player is in field of view && Close enought to be killed
        if (fieldOfView.IsTarget && Vector2.Distance(transform.position, target.position) <= PlayerDeadDistance)
        {
            _playerIsDead = true;
            agent.isStopped = true;
        }
        //Player is in field of view
        else if (fieldOfView.IsTarget)
        {
            nextPoint = null;
            agent.SetDestination(target.position);
            agent.speed = _enemyChaseSpeed;
            _lastPLayerPosition = target.position;
        }
        //Player in not in field of view but the place where he was is known
        else if (!fieldOfView.IsTarget && _isChasingPlayer)
        {
            Debug.Log(Vector2.Distance(transform.position, _lastPLayerPosition));

            if (Vector2.Distance(transform.position, _lastPLayerPosition) <= PatrolStopDistance)
            {
                _isChasingPlayer = false;
                Debug.Log(Vector2.Distance(transform.position, _lastPLayerPosition));
            }
            agent.SetDestination(_lastPLayerPosition);
        }
        else
        {
            Debug.Log("WTH is this");
        }
    }

    private void SmoothRotate2D()
    {
        if (agent.velocity.magnitude > 0.1f) // Only rotate if the agent is moving
        {
            Vector2 direction = agent.velocity.normalized; // Calculate the movement direction

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;             // Calculate the target angle based on direction
            float currentAngle;
            if (fieldOfView.IsTarget)
                currentAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * chaseRotationRate);
            else
                currentAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * normalRotationRate);

            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }
    private void RapidRotate2d()
    {
        Vector3 targetPos = target.position;
        Vector3 thisPos = transform.position;
        targetPos.x = targetPos.x - thisPos.x;
        targetPos.y = targetPos.y - thisPos.y;
        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 0.5f));
    }



    private Transform GetNextPathPoint()
    {
        Debug.Log("GetNext POINTTT");
        var nextPoint = _wayPoints.Dequeue();
        _wayPoints.Enqueue(nextPoint);
        return nextPoint;
    }

}
