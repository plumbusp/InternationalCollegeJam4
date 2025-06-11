using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyPerceptionAI enemyPerceptionAI;
    [SerializeField] protected EnemyMovement enemyMovement;
    [SerializeField] protected EnemyParameters enemyParameters;

    protected abstract void Start();
    protected virtual void LateUpdate()
    {
        enemyPerceptionAI.Detect();
    }
    protected virtual void HandleTargetKill(Transform target)
    {
        Debug.Log($"Target marked as{target.tag} was killed ");
        enemyMovement.isAllowedToMove = false;
        enemyPerceptionAI.isAllowedToDetect = false;
    }

    protected abstract bool DetectionLimited(Transform transform);
}
