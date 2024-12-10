using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class OldFieldOfView : MonoBehaviour
{
    [SerializeField] private int rayCount = 2;
    [SerializeField] private float fieldOV = 90f; //angle degrees
    [SerializeField] private float viewDistance = 50f;
    [SerializeField] private LayerMask detectionLayer;

    private Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        Vector3 origin = Vector2.zero;
        float currentAngle = 0f;
        float angleIncrease = fieldOV / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D hit = Physics2D.Raycast(origin, GetVectorFromAngle(currentAngle), viewDistance, detectionLayer);

            if (hit.collider == null)
            {
                vertex = origin + GetVectorFromAngle(currentAngle) * viewDistance;
            }
            else
            {
                Debug.Log("HitPoint!!");
                vertex = hit.point;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;
            currentAngle -= angleIncrease;
        }

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    private static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / (180f));
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = Vector2.zero;
        float currentAngle = 0f;
        float angleIncrease = fieldOV / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D hit = Physics2D.Raycast(origin, GetVectorFromAngle(currentAngle), viewDistance, detectionLayer);

            if (hit.collider == null)
            {
                vertex = origin + GetVectorFromAngle(currentAngle) * viewDistance;
                Gizmos.DrawLine(origin, vertex);
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.DrawLine(origin, hit.point);
            }
        }

    }
}
  
