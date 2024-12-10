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
    [SerializeField] private float _timeOnInvestigation;
    private WaitForSeconds wait;

    //NavMesh
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float PatrolStopDistance = 1f;


    //Path 
    [Header("Path")]
    [SerializeField] private List<Transform> _wayPointsList;
    private Queue<Transform> _wayPoints = new Queue<Transform>();
    //private Vector3 nextPoint;

    //Sounds
    [Header("Sound making items (must contain ISoundMaker interface)")]
    [SerializeField] private List<GameObject> _soundsMakers;
    private bool _Investigating = false;
    private bool _waitingOnPlace = false;

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        wait = new WaitForSeconds(_timeOnInvestigation);
        agent.speed = _enemyNormalSpeed;

        foreach (var t in _wayPointsList)
        {
            _wayPoints.Enqueue(t);
            if (t == null)
                Debug.LogWarning("Way Point cannot be null !!!! >:(");
        }

        foreach (var maker in _soundsMakers)
        {
            var soundM = maker.GetComponent<ISoundMaker>();
            soundM.OnLoudSoundMade += HandleLoudSound;
            soundM.OnQuiteSoundMade += HandleQuiteSound;
        }
        agent.SetDestination(GetNextPathPoint().position);
    }

    private void Update()
    {
        SmoothRotate2D();

        if (_waitingOnPlace || _Investigating)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _closeHearingRadius);
            if(colliders.Length > 0 )
            {
                foreach (var collider in colliders)
                {
                    if(collider.tag == "Player")
                    {
                        Debug.Log("Player LOST!!!");
                    }
                }
            }
        }

        if(_waitingOnPlace)
            return;

        if(agent.remainingDistance <= PatrolStopDistance)
        {
            if (_Investigating && !_waitingOnPlace)
            {
                _waitingOnPlace= true;
                StartCoroutine(WaitOnInvestigation());
                return;
            }
            agent.SetDestination(GetNextPathPoint().position);
            agent.speed = _enemyNormalSpeed;
        }
    }

    private void HandleLoudSound(Vector3 fromWhere)
    {
        agent.speed = _enemyChaseSpeed;
        agent.SetDestination(fromWhere);
        _Investigating = true;
        Debug.Log("Going to investigate loud sound");
    }
    private void HandleQuiteSound(Vector3 fromWhere)
    {
        Vector3 direction = fromWhere - transform.position;
        Quaternion.LookRotation(direction, Vector3.up);
        agent.speed = _enemyChaseSpeed;
        agent.SetDestination(fromWhere);
        _Investigating = true;

        Debug.Log("I hear something...");
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

    private IEnumerator WaitOnInvestigation()
    {
        _waitingOnPlace = true;
        agent.isStopped = true;
        yield return wait;
        agent.isStopped = false;
        _Investigating = false;
        _waitingOnPlace = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, _closeHearingRadius);
        Gizmos.color = Color.yellow;
    }
}
