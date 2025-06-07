using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyParameters", menuName = "Enemy/EnemyParameters", order =0)]
public class EnemyParameters : ScriptableObject
{
    public List<string> DetectionTags = new List<string>();
    public float MouseEatingTime;
}
