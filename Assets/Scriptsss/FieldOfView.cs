using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private Material _VisionConeMaterial;
    [SerializeField] private float _VisionRange;
    [SerializeField] private float _VisionAngle;
    [SerializeField] private LayerMask _VisionObstructingLayer; //layer with objects that obstruct the enemy view, like walls, for example
    [SerializeField] private int _VisionConeResolution = 120; //the vision cone will be made up of triangles, the higher this value is the pretier the vision cone will be
    
    private Mesh _VisionConeMesh;
    private MeshFilter _meshFilter;

    //private Quaternion _leftRotation = Quaternion.Euler(0, 0, 90);
    //private Quaternion _rightRotation = Quaternion.Euler(0, 0, 270);
    //private Quaternion _upRotation =  Quaternion.Euler(0, 0, 0);
    //private Quaternion _downRotation = Quaternion.Euler(0, 0, 180);


    int[] triangles;
    Vector3[] Vertices;
    float Currentangle;
    float angleIcrement;
    float Sine;
    float Cosine;

    void Start()
    {
        transform.AddComponent<MeshRenderer>().material = _VisionConeMaterial;
        _meshFilter = transform.AddComponent<MeshFilter>();
        _VisionConeMesh = new Mesh();
        _VisionAngle *= Mathf.Deg2Rad;
    }


    void Update()
    {
        DrawVisionCone();//calling the vision cone function everyframe just so the cone is updated every frame
    }

    void DrawVisionCone()//this method creates the vision cone mesh
    {
        triangles = new int[(_VisionConeResolution - 1) * 3];
        Vertices = new Vector3[_VisionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        Currentangle = -_VisionAngle / 2;
        angleIcrement = _VisionAngle / (_VisionConeResolution - 1);

        for (int i = 0; i < _VisionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.up * Cosine) + (transform.right * Sine);
            Vector3 VertForward = (Vector3.up * Cosine) + (Vector3.right * Sine);
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, RaycastDirection, _VisionRange, _VisionObstructingLayer);
            if (raycastHit)
            {
                Vertices[i + 1] = VertForward * raycastHit.distance;
            }
            else
            {
                Vertices[i + 1] = VertForward * _VisionRange;
            }


            Currentangle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        _VisionConeMesh.Clear();
        _VisionConeMesh.vertices = Vertices;
        _VisionConeMesh.triangles = triangles;
        _meshFilter.mesh = _VisionConeMesh;
    }

    //public void ChangeViewDirection(WayPoint referencePoint)
    //{
    //    switch (referencePoint.CharacterDirection)
    //    {
    //        case CharacterDirection.Left:
    //            transform.rotation = _leftRotation;
    //            break;

    //        case CharacterDirection.Right:
    //            transform.rotation = _rightRotation;
    //            break;

    //        case CharacterDirection.Up:
    //            transform.rotation = _upRotation;
    //            break;

    //        case CharacterDirection.Down:
    //            transform.rotation = _downRotation;
    //            break;
    //    }
    //}
}
