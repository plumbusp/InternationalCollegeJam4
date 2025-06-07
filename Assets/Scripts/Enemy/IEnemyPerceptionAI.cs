using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyPerceptionAI
{
    /// <summary>
    /// Requires constant calls to find out the current situation.
    /// </summary>
    /// 
    public void Initialize(EnemyParameters enemyParameters);
    public void Detect();
    public Action OnTargetDetected {  get; set; }
}
