using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private FieldOfView _EnemyFieldOfView;

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

    private float _timePassed;
    [SerializeField] private float _LerpSpeed;

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

            //Rotating object to look in the directin of next Pos 
            var direction = transform.position - nextPoint.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(Quaternion.AngleAxis(angle, transform.forward), transform.rotation, _timePassed);
            _timePassed += Time.deltaTime;
        }
        else
        {
            transform.position = nextPoint.transform.position;
            _timePassed = 0;
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
