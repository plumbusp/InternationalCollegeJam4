using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterDirection
{
    Up,
    Down,
    Left,
    Right
}

public class WayPoint : MonoBehaviour
{
    [SerializeField] private CharacterDirection _characterDirection;
    [SerializeField] private Vector3 _TargetRotation;
    public CharacterDirection CharacterDirection {  get { return _characterDirection; } }
    public Vector3 TargetRotation {  get { return _TargetRotation; } }
}
