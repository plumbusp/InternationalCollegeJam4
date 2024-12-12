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

    [Header("Mouse Settings")]
    [SerializeField] private Mouse mouse;
    [SerializeField] private float mouseEatingTime = 3f;

    private WaitForSeconds mouseEatingDelay;
    private Transform currentTarget;

    private bool isPlayerDead = false;
    private bool isWaiting = false;

    [SerializeField] private Transform _rotationTarget;
    private Vector3 _initialPosition;
    private float _initialZRotation;
    private bool _onInitialRotation;

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = normalSpeed;


        fieldOfView.OnPlayerDetected += HandlePlayerDetection;
        fieldOfView.OnMouseDetected += HandleMouseDetection;

        mouseEatingDelay = new WaitForSeconds(mouseEatingTime);

        _initialPosition = transform.position;
        _initialZRotation = transform.eulerAngles.z;
    }

    private void Update()
    {
        UpdateFieldOfView();
        if (isPlayerDead || isWaiting) return;
        SmoothRotateTowardsMovement();

        if (currentTarget != null)
        {
            SmoothRotateTowardsMovement();
            ProcessTargetBehavior();
        }
        else if (Vector2.Distance(transform.position, _initialPosition) > patrolStopDistance)
        {
            SmoothRotateTowardsMovement();
            agent.SetDestination(_initialPosition);
        }
        else if (!_onInitialRotation)
        {
            RotateAgentTowardsTarget(_rotationTarget);
        }
        else
        {
            Debug.Log("Just chil bro");
        }
    }


    #region Field of View
    private void UpdateFieldOfView()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.right);
    }
    #endregion

    #region Target Handling
    private void HandlePlayerDetection()
    {
        if (player.InSafeSpot || isPlayerDead) return;

        SetTarget(player.transform, chaseSpeed);
    }

    private void HandleMouseDetection()
    {
        player.InSafeSpot = true;
        SetTarget(mouse.transform, chaseSpeed);
    }

    private void SetTarget(Transform target, float speed)
    {
        currentTarget = target;
        agent.speed = speed;
    }

    private void ProcessTargetBehavior()
    {
        agent.SetDestination(currentTarget.position);

        if (currentTarget == player.transform)
        {
            if (IsInRange(player.transform, deathRange))
            {
                HandlePlayerCaught();
            }
            else if (!fieldOfView.IsTarget)
            {
                currentTarget = null;
                return;
            }
        }
        else if (currentTarget == mouse.transform)
        {
            Debug.Log(IsInRange(mouse.transform, deathRange) + "   " + Vector2.Distance(transform.position, currentTarget.position));
            if (IsInRange(mouse.transform, deathRange))
            {
                StartCoroutine(HandleMouseInteraction());
            }
        }
    }
    #endregion

    #region Target Interaction
    private bool IsInRange(Transform target, float range)
    {
        return Vector2.Distance(transform.position, target.position) <= range;
    }

    private void HandlePlayerCaught()
    {
        isPlayerDead = true;
        agent.isStopped = true;
        ScreensLogic.Instance.ShowDeadScreen();
        Debug.Log("Player LOST!");
    }

    private IEnumerator HandleMouseInteraction()
    {
        isWaiting = true;
        currentTarget = null;

        yield return mouseEatingDelay;

        isWaiting = false;
        player.InSafeSpot = false;
        mouse.Eat();
    }
    #endregion


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

    private void RotateAgentTowardsTarget(Transform target)
    {
        if (target == null) return;

        // Calculate the direction towards the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Ignore vertical differences if needed
        direction.z = 0;

        // Calculate the desired rotation
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        // Smoothly interpolate rotation (optional)
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothRotationSpeed * 100
        );
    }
}