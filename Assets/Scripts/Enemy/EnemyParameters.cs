using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyParameters", menuName = "Enemy/EnemyParameters")]
public class EnemyParameters : ScriptableObject
{
    public List<string> DetectionTags = new List<string>();
}
