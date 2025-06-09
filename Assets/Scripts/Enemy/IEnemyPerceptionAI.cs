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
    public abstract void Initialize(EnemyParameters enemyParameters, Transform enemyTransform, Func<Transform, bool> detectionLimiter);
    public abstract void Detect();
    public abstract Action<Transform> OnTargetDetected {  get; set; }
    public abstract Action OnTargetLost {  get; set; }
    public abstract Action<Transform> OnTargetCanBeKilled { get; set; }
    public abstract bool isAllowedToDetect { get; set; }
}
