using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindEnemy : Enemy
{
    protected override void Start()
    {
        enemyPerceptionAI.Initialize(enemyParameters, transform, DetectionLimited);
        enemyPerceptionAI.isAllowedToDetect = true;
        enemyPerceptionAI.OnTargetDetected += (Transform target) => enemyMovement.Chase(target);
        enemyPerceptionAI.OnTargetLost += enemyMovement.LookAround;
        enemyPerceptionAI.OnTargetCanBeKilled += HandleTargetKill;

        enemyMovement.Intialize(enemyParameters);
        enemyMovement.isAllowedToMove = true;
        enemyMovement.Patrol();
    }

    /// <summary>
    /// Returns true if detection can't be perform, false otherwise
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected override bool DetectionLimited(Transform transform)
    {
        return false; // Blind enemies can detect targets without limitations
    }
}
