using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPerceptionAI: MonoBehaviour
{
    protected Action<Transform> _onTargetDetected;
    protected Action _onTargetLost;
    protected Action<Transform> _onTargetCanBeKilled;

    protected EnemyParameters enemyParameters;
    protected Transform enemyTransform;
    protected Func<Transform, bool> detectionLimiter;

    public virtual void Initialize(EnemyParameters enemyParameters, Transform enemyTransform, Func<Transform, bool> detectionLimiter)
    {
        this.enemyParameters = enemyParameters;
        this.enemyTransform = enemyTransform;
        this.detectionLimiter = detectionLimiter;
    }
    /// <summary>
    /// For smoother work should be called from LateUpdate
    /// </summary>
    public abstract void Detect();
    public abstract Action<Transform> OnTargetDetected {  get; set; }
    public abstract Action OnTargetLost {  get; set; }
    public abstract Action<Transform> OnTargetCanBeKilled { get; set; }
    public abstract bool isAllowedToDetect { get; set; }

    protected bool CheckForTargetTag(string tagName)
    {
        if (enemyParameters.DetectionTags.Contains(tagName))
            return true;
        return false;
    }
}
