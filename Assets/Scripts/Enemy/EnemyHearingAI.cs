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

    private int _currentSoundPriority;
    private int _nonePriority = 0;
    private int _lowPriority = 1;
    private int _normalPriority = 2;
    private int _urgentPriority = 3;

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
            soundM.OnSuperLoudSoundMade += HandleSuperLoudSound;
            soundM.OnLoudSoundMade += HandleLoudSound;
            soundM.OnQuiteSoundMade += HandleQuiteSound;
        }

        _currentSoundPriority = _nonePriority;
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
                        ScreensLogic.Instance.ShowDeadScreen();
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

    private void HandleSuperLoudSound(Vector3 fromWhere)
    {
        if (!CheckSoundPriority(_urgentPriority))
            return;

        agent.isStopped = false;
        agent.speed = _enemyChaseSpeed;
        agent.SetDestination(fromWhere);
        _Investigating = true;
        Debug.Log("Now what was THAT ?!!");
    }

    private void HandleLoudSound(Vector3 fromWhere)
    {
        if(!CheckSoundPriority(_normalPriority))
            return;

        agent.isStopped = false;
        agent.speed = _enemyChaseSpeed;
        agent.SetDestination(fromWhere);
        _Investigating = true;
        Debug.Log("Going to investigate loud sound ");
    }
    private void HandleQuiteSound(Vector3 fromWhere)
    {
        if (!CheckSoundPriority(_lowPriority))
            return;

        if (Vector2.Distance(transform.position, fromWhere) <= _closeHearingRadius)
        {
            agent.isStopped = false;
            Vector3 direction = fromWhere - transform.position;
            Quaternion.LookRotation(direction, Vector3.up);
            agent.speed = _enemyChaseSpeed;
            agent.SetDestination(fromWhere);
            _Investigating = true;

            Debug.Log("I hear something...");
        }
        else
        {
            //Debug.Log("Meh, not bothered");
        }
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
        _currentSoundPriority = _nonePriority;
    }

    /// <summary>
    /// Returns true if new sound priority is higher then current sound priority
    /// </summary>
    /// <returns></returns>
    private bool CheckSoundPriority(int soundPriority)
    {
        if(_currentSoundPriority <= soundPriority)
        {
            _currentSoundPriority = soundPriority;
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _closeHearingRadius);
        Gizmos.color = Color.yellow;
    }
}
