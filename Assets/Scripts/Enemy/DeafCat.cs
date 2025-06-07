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
        enemyVisionAI.Initialize(deafCatParameters, transform);
        enemyVisionAI.OnTargetDetected += (Transform target) => EnemyMovement.Chase(target);
        enemyVisionAI.OnTargetLost += EnemyMovement.LookAround;

        EnemyMovement.Intialize(deafCatParameters);
        EnemyMovement.Patrol();
        //Subsribe on enemy catched player
    }
    private void LateUpdate()
    {
        enemyVisionAI.Detect();
    }
}
