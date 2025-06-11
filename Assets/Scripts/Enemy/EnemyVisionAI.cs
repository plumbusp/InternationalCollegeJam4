using UnityEngine;
using CodeMonkey.Utils;
using System;
using UnityEngine.UIElements;

public class EnemyVisionAI : IEnemyPerceptionAI
{
    private Action<Transform> _onTargetDetected;
    private Action _onTargetLost;
    private Action<Transform> _onTargetCanBeKilled;
    override public Action<Transform> OnTargetDetected { get => _onTargetDetected; set => _onTargetDetected = value; }
    override public Action OnTargetLost { get => _onTargetLost; set => _onTargetLost = value; }
    public override Action<Transform> OnTargetCanBeKilled { get => _onTargetCanBeKilled; set => _onTargetCanBeKilled = value; }

    private bool _isAllowedToDetect;
    public override bool isAllowedToDetect 
    {
        get 
        {
            return _isAllowedToDetect;
        }
        set
        {
            _isAllowedToDetect = value;
        }
    }

    // Field of View Visuals
    Mesh mesh;

    Vector3 origin = Vector3.zero;

    float startAngle;

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] int rayCount = 2;
    [SerializeField] float fieldOfView = 90f;
    [SerializeField] float distance = 50f;
    [SerializeField] Vector3 offset;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float noticeCoolDown;
    // Field of View Visuals

    private EnemyParameters enemyParameters;
    private Transform enemyTransform;
    public bool IsTarget { get; private set; }

    private bool _detected;
    private bool _isSeeingTaget;
    private Func<Transform, bool> detectionLimiter;

    override public void Initialize(EnemyParameters enemyParameters, Transform enemyTransform, Func<Transform, bool> detectionLimiter)
    {
        this.enemyParameters = enemyParameters;
        this.enemyTransform = enemyTransform;
        this.detectionLimiter = detectionLimiter;

        mesh = new Mesh();
        meshFilter.mesh = mesh;
        _isSeeingTaget = false;
    }


    /// <summary>
    /// For smoother work should be called from LateUpdate
    /// </summary>
    override public void Detect()
    {
        IsTarget = false;

        SetOrigin(enemyTransform.position);
        SetDirection(enemyTransform.right);

        // creating field of view visuals
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        var angle = startAngle;
        var angleIncrease = fieldOfView / rayCount;

        var vertices = new Vector3[rayCount + 1 + 1];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[rayCount * 3];

        var vertexIndex = 1;
        var trianglesIndex = 0;

        vertices[0] = origin;
        // creating field of view visuals

        for (int i = 0; i <= rayCount; i++)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(angle), distance, layerMask);
            Vector2 vertex = raycastHit2D.collider ? raycastHit2D.point : origin + MathHelper.AngleToVector2D(angle + transform.eulerAngles.y) * distance;

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[trianglesIndex + 0] = 0;
                triangles[trianglesIndex + 1] = vertexIndex - 1;
                triangles[trianglesIndex + 2] = vertexIndex;

                trianglesIndex += 3;
            }

            vertexIndex++;

            angle -= angleIncrease;

            mesh.RecalculateBounds();

            if (!_isAllowedToDetect)
                continue;

            _detected = (raycastHit2D.collider != null) && (CheckForTargetTag(raycastHit2D.collider.tag));

            if (_detected)
            {
                if (detectionLimiter(raycastHit2D.collider.transform))
                    continue;

                var collidersInRange = Physics2D.OverlapCircleAll(origin, enemyParameters.deathRange);
                foreach (var collider in collidersInRange)
                {
                    if (CheckForTargetTag(collider.transform.tag))
                    {
                        OnTargetCanBeKilled?.Invoke(collider.transform);
                        break;
                    }
                }
            }

            //States and events handling
            if (_detected && (_isSeeingTaget == false))
            {
                _isSeeingTaget = true;
                _onTargetDetected?.Invoke(raycastHit2D.collider.transform);
            }
            else if (!_detected && _isSeeingTaget == true)
            {
                _isSeeingTaget = false;
                _onTargetLost?.Invoke();
            }
            //States and events handling
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    private void SetOrigin(Vector3 origin)
    {
        Debug.Log(origin);
        this.origin = origin + offset;
    }

    private void SetDirection(Vector3 direction)
    {
        startAngle = MathHelper.VectorToAngle2D(direction) + fieldOfView / 2f;
    }

    private bool CheckForTargetTag(string tagName)
    {
        if (enemyParameters.DetectionTags.Contains(tagName))
            return true;
        return false;
    }
}