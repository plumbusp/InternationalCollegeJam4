using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Field Of View")]
    [SerializeField] private FieldOfView _EnemyFieldOfView;
    [SerializeField] private Transform _origin;

    [Space (30f)]
    [Header ("Rotation Placeholders")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _UpRotationSprite;
    [SerializeField] private Sprite _DownRotationSprite;
    [SerializeField] private Sprite _RightRotationSprite;
    [SerializeField] private Sprite _LeftRotationSprite;


    [SerializeField] private List<WayPoint> waypoints;
    private Queue<WayPoint> _wayPointsQueue = new Queue<WayPoint>();

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minReachDistance = 0.05f; 

    private WayPoint nextPoint; //The point that transform is currently moving towards

    private Vector3 _currentAngle; // used in rotation

    private void Start()
    {
        foreach (var waypoint in waypoints)
        {
            _wayPointsQueue.Enqueue(waypoint); 
        }
        nextPoint = GetNextWayPoint();
        Rotate(nextPoint);
    }

    private void Update()
    {
        Move();
        _EnemyFieldOfView.SetOrigin(_origin.position);
    }

    private WayPoint GetNextWayPoint()
    {
        var nextP = _wayPointsQueue.Dequeue();
        _wayPointsQueue.Enqueue(nextP);
        return nextP;
    }
    private void Move()
    {
        if (nextPoint == null)
        {
            Debug.LogError("Next Point Was't Initialized!");
            return;
        }

        if (Vector2.Distance(transform.position, nextPoint.transform.position) > minReachDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position,
               nextPoint.transform.position,
               moveSpeed * Time.deltaTime);

        }
        else
        {
            transform.position = nextPoint.transform.position;
            nextPoint = GetNextWayPoint();
            Rotate(nextPoint);
        }
    }

    /// <summary>
    /// "Rotates" the Enemy by enabling correcponing spites based on point requarements
    /// </summary>
    /// <param name="nextPoint"></param>
    private void Rotate(WayPoint nextPoint)
    {
        switch (nextPoint.CharacterDirection)
        {
            case CharacterDirection.Left:
                _spriteRenderer.sprite = _LeftRotationSprite;
                break;

            case CharacterDirection.Right:
                _spriteRenderer.sprite = _RightRotationSprite;
                break;

            case CharacterDirection.Up:
                _spriteRenderer.sprite = _UpRotationSprite;
                break;

            case CharacterDirection.Down:
                _spriteRenderer.sprite = _DownRotationSprite;
                break;
        }
    }
}
