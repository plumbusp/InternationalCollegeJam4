using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyPerceptionAI enemyVisionAI;
    [SerializeField] protected EnemyMovement enemyMovement;
    [SerializeField] protected EnemyParameters enemyParameters;

   private void LateUpdate()
   {
        enemyVisionAI.Detect();
   }
    protected void HandleTargetKill(Transform target)
    {
        Debug.Log($"Target marked as{target.tag} was killed ");
        enemyMovement.isAllowedToMove = false;
        enemyVisionAI.isAllowedToDetect = false;
    }

    protected abstract bool DetectionLimited(Transform transform);
}
