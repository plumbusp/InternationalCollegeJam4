using UnityEngine;

public class DeafCat : Enemy
{
    //Enemy movement script 
    private void Start()
    {
        enemyVisionAI.Initialize(enemyParameters, transform, DetectionLimited);
        enemyVisionAI.isAllowedToDetect = true;
        enemyVisionAI.OnTargetDetected += (Transform target) => enemyMovement.Chase(target);
        enemyVisionAI.OnTargetLost += enemyMovement.LookAround;
        enemyVisionAI.OnTargetCanBeKilled += HandleTargetKill;

        enemyMovement.Intialize(enemyParameters);
        enemyMovement.isAllowedToMove = true;
        enemyMovement.Patrol();
        //Subsribe on enemy catched player
    }
    
    /// <summary>
    /// Returns true if detection can't be perform, false otherwise
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    override protected bool DetectionLimited(Transform transform)
    {
        if (transform.TryGetComponent(out IEnemyTarget enemyTarget))
        {
            if (enemyTarget.InSafeSpot)
                return true;
        }
        return false;
    }
}
