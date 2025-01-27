﻿using UnityEngine;
using CodeMonkey.Utils;
using System;

public class FieldOfView : MonoBehaviour {

    public Action OnPlayerDetected;
    public Action OnMouseDetected;

    Mesh mesh;

    Vector3 origin = Vector3.zero;

    float startAngle;

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] int rayCount = 2;
    [SerializeField] float fieldOfView = 90f;
    [SerializeField] float distance = 50f;
    [SerializeField] Vector3 offset;
    [SerializeField] string targetTag;
    [SerializeField] string mouseTag;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float noticeCoolDown;

    public bool IsTarget { get; private set; }

    private void Start()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    private void LateUpdate()
    {
        IsTarget = false;

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

        for (int i = 0; i <= rayCount; i++)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(angle), distance, layerMask);
            Vector2 vertex = raycastHit2D.collider ? raycastHit2D.point : origin + MathHelper.AngleToVector2D(angle + transform.eulerAngles.y) * distance;

            if (raycastHit2D.collider != null) 
            {
                if(raycastHit2D.collider.tag == targetTag)
                {
                    OnPlayerDetected?.Invoke();
                    IsTarget = true;
                }
                else if(raycastHit2D.collider.tag == mouseTag)
                {
                    OnMouseDetected?.Invoke();
                }
            }


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
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin + offset;
    }

    public void SetDirection(Vector3 direction)
    {
        startAngle = MathHelper.VectorToAngle2D(direction) + fieldOfView / 2f;
    }

    bool CompareLayer(LayerMask layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}