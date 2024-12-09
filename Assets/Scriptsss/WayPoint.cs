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
    public CharacterDirection CharacterDirection {  get { return _characterDirection; } }
}
