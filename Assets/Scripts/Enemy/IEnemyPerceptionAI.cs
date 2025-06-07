using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEnemyPerceptionAI: MonoBehaviour
{
    /// <summary>
    /// Requires constant calls to find out the current situation.
    /// </summary>
    /// 
    public abstract void Initialize(EnemyParameters enemyParameters, Transform enemyTransform);
    public abstract void Detect();
    public abstract Action<Transform> OnTargetDetected {  get; set; }
    public abstract Action OnTargetLost {  get; set; }
}
