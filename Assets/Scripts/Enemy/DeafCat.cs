using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyMovement;

public class DeafCat : MonoBehaviour
{

    [SerializeField] private IEnemyPerceptionAI enemyVisionAI;
    [SerializeField] private EnemyParameters deafCatParameters;
    [SerializeField] private EnemyMovement EnemyMovement;

    //Enemy movement script 
    private void Start()
    {
        enemyVisionAI.Initialize(deafCatParameters, transform, IsDetectionLimited);
        enemyVisionAI.isAllowedToDetect = true;
        enemyVisionAI.OnTargetDetected += (Transform target) => EnemyMovement.Chase(target);
        enemyVisionAI.OnTargetLost += EnemyMovement.LookAround;
        enemyVisionAI.OnTargetCanBeKilled += HandleTargetKill;

        EnemyMovement.Intialize(deafCatParameters);
        EnemyMovement.isAllowedToMove = true;
        EnemyMovement.Patrol();
        //Subsribe on enemy catched player
    }
    private void LateUpdate()
    {
        enemyVisionAI.Detect();
    }
    private void HandleTargetKill(Transform target)
    {
        Debug.Log($"Target marked as{ target.tag} was killed " );
        EnemyMovement.isAllowedToMove = false;
        enemyVisionAI.isAllowedToDetect = false;
    }
    /// <summary>
    /// Returns true if detection can't be perform, false otherwise
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    private bool IsDetectionLimited(Transform transform)
    {
        if (transform.TryGetComponent(out IEnemyTarget enemyTarget))
        {
            if (enemyTarget.InSafeSpot)
                return true;
        }
        return false;
    }
}
